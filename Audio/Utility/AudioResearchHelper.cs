﻿using Audio.AudioEditor;
using Audio.FileFormats.WWise.Hirc.Shared;
using Audio.FileFormats.WWise.Hirc.V136;
using Audio.Storage;
using Serilog;
using Shared.Core.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Audio.Utility
{
    public class AudioResearchHelper
    {
        ILogger _logger = Logging.Create<AudioResearchHelper>();
        private readonly IAudioRepository _audioRepository;

        public AudioResearchHelper(IAudioRepository audioRepository)
        {
            _audioRepository = audioRepository;
        }

        public void ExportNamesToFile(string path, bool openFile = false)
        {
            var stringbuilder = new StringBuilder();
            foreach (var item in _audioRepository.NameLookUpTable)
                stringbuilder.AppendLine($"{item.Key}, {item.Value}");

            File.WriteAllText(path, stringbuilder.ToString());

            Process.Start("explorer.exe", "c:\\temp");
        }

        public void ExportDialogEventsToFile(CAkDialogueEvent_v136 dialogEvent, bool openFile = false)
        {
            if (dialogEvent == null)
            {
                _logger.Here().Warning("Error Converting to CSV - DialogEvent not correct version");
                return;
            }

            var numArgs = dialogEvent.ArgumentList.Arguments.Count();// - 1
            var root = dialogEvent.AkDecisionTree.Root;
            if (numArgs == 0)
                throw new Exception("Error Converting to CSV - No arguments in DialogEvent");

            var table = new List<string>();
            foreach (var children in root.Children)
                GenerateRow(children, 0, numArgs, new Stack<string>(), table, _audioRepository);

            var keys = dialogEvent.ArgumentList.Arguments.Select(x => _audioRepository.GetNameFromHash(x.ulGroupId)).ToList();
            var prettyKeys = string.Join("|", keys);
            prettyKeys += "|WWiseChild";
            var prettyTable = table.Select(x => string.Join("|", x)).ToList();

            var ss = new StringBuilder();
            ss.AppendLine("sep=|");
            ss.AppendLine(prettyKeys);
            foreach (var row in prettyTable)
                ss.AppendLine(row);

            var wholeFile = ss.ToString();
            File.WriteAllText("F:\\DialogueEvent.csv", wholeFile);
        }

        static void GenerateRow(AkDecisionTree.Node currentNode, int currentArgrument, int numArguments, Stack<string> pushList, List<string> outputList, IAudioRepository audioRepository)
        {
            var currentNodeContent = audioRepository.GetNameFromHash(currentNode.Key);
            pushList.Push(currentNodeContent);

            bool isDone = numArguments == currentArgrument + 1;
            if (isDone)
            {
                var currentLine = pushList.ToArray().Reverse().ToList();
                currentLine.Add(audioRepository.GetNameFromHash(currentNode.AudioNodeId));  // Add the wwise child node
                outputList.Add(string.Join("|", currentLine));
            }
            else
            {
                foreach (var child in currentNode.Children)
                    GenerateRow(child, currentArgrument + 1, numArguments, pushList, outputList, audioRepository);
            }

            pushList.Pop();
        }

        public void CustomExportDialogEventsToFile(CAkDialogueEvent_v136 dialogEvent, bool openFile = false)
        {
            if (dialogEvent == null)
            {
                _logger.Here().Warning("Error Converting to CSV - DialogEvent not correct version");
                return;
            }

            var numArgs = dialogEvent.ArgumentList.Arguments.Count();// - 1
            var root = dialogEvent.AkDecisionTree.Root;
            if (numArgs == 0)
                throw new Exception("Error Converting to CSV - No arguments in DialogEvent");

            var gkeys = dialogEvent.ArgumentList.Arguments.Select(x => _audioRepository.GetNameFromHash(x.ulGroupId)).ToList();
            var table = new List<string>();
            GenerateCustomRow(root, -1, 0, gkeys, table, _audioRepository);
            var prettyKeys = "Key|uWeight|uProbability|IsAudioNode|audioNodeName|parentId";

            var ss = new StringBuilder();
            ss.AppendLine(prettyKeys);
            foreach (var row in table)
                ss.AppendLine(row);

            var wholeFile = ss.ToString();
            File.WriteAllText("F:\\DialogueEvent.csv", wholeFile);
        }
        static void GenerateCustomRow(AkDecisionTree.Node cNode, int pId, int depth, List<string> gkeys, List<string> outputList, IAudioRepository audioRepository)
        {
            var currentNodeContent = audioRepository.GetNameFromHash(cNode.Key);
            if (currentNodeContent == "0")
            {
                currentNodeContent = "Any";
            }
            var audioNodeName = $"{cNode.AudioNodeId}";
            string keyName;
            if (cNode.IsAudioNode())
            {
                audioNodeName = $"{audioRepository.GetNameFromHash(cNode.AudioNodeId)}({cNode.AudioNodeId})";
                keyName = $"{currentNodeContent}({cNode.Key})";
            }
            else
            {
                keyName = $"{gkeys[depth]} == {currentNodeContent}({cNode.Key})";
            }
            outputList.Add($"{keyName}|{cNode.uWeight}|{cNode.uProbability}|{cNode.IsAudioNode()}|{audioNodeName}|{pId}");
            var cId = outputList.Count - 1;
            foreach (var child in cNode.Children)
                GenerateCustomRow(child, cId, depth + 1, gkeys, outputList, audioRepository);
        }


        public void GenerateActorMixerTree(string filename = "actorTree.txt")
        {
            var mixers = _audioRepository.GetAllOfType<CAkActorMixer_v136>();

            var allRootNodes = mixers
                .Where(x => x.NodeBaseParams.DirectParentID == 0)
                .DistinctBy(x => x.Id)
                .ToList();

            var mixerTrees = allRootNodes.Select(x => new WWiseTreeParserChildren(_audioRepository, true, true, false).BuildHierarchy(x)).ToList();
            var ss = new StringBuilder();
            foreach (var mixer in mixerTrees)
                ConvertToString(mixer, 0, ss);

            var wholeDataString = ss.ToString();
            var lines = wholeDataString.Split('\n').Count();
            File.WriteAllText($"D:\\Research\\Audio\\{filename}", wholeDataString);
        }


        void ConvertToString(HircTreeItem item, int indentation, StringBuilder ss)
        {
            if (item.Item == null)
                return;

            var itemName = item.DisplayName;
            if (item.Item is CAkActorMixer_v136 actorMixer)
                itemName = $"ActorMixer {_audioRepository.GetNameFromHash(actorMixer.Id)} - Bus:{_audioRepository.GetNameFromHash(actorMixer.NodeBaseParams.OverrideBusId)}";
            else
                return;
            ss.AppendLine($"{new string('\t', indentation)}{itemName}");
            foreach (var childItem in item.Children)
                ConvertToString(childItem, indentation + 1, ss);
        }


        // List all sounds
        // Find owning event (parent, parent, parent)
        // Find override bus id for the parent graph override busId != 0 
    }
}


// For all dialog events
// For all events 
// Event => Action => Switch =>
//                              DecratatorNode : SwitchCase
//                                      Sound
//                                      Sound
