﻿using AssetEditor;
using AssetEditor.Services;
using Audio.Storage;
using SharedCore.PackFiles;

namespace AudioResearch
{
    internal class LotrDataLoading
    {
        public void Run()
        {

            using var application = new SimpleApplication(false);

            var pfs = application.GetService<PackFileService>();
            pfs.Load(@"C:\Users\ole_k\Downloads\attila_bnks.pack", true, true);

            var audioRepo = application.GetService<IAudioRepository>();


        }
    }
}
