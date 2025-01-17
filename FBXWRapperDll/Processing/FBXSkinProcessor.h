#pragma once

#include <fbxsdk.h>
#include "..\DataStructures\PackedMeshStructs.h"
#include "..\Helpers\tools.h"
#include "..\Logging\Logging.h"

namespace wrapdll
{
	class FBXSkinProcessorService
	{
	public:
		/// <summary>
		/// Process 
		/// </summary>
		/// <param name="_poSourceFbxMesh"></param>
		/// <param name="destPackedMesh"></param>
		/// <param name="boneTable"></param>
		/// <param name="controlPointInfluences"></param>
		/// <returns></returns>
		static bool ProcessSkin(
			FbxMesh* _poSourceFbxMesh,
			PackedMesh& destPackedMesh,
			const std::vector < std::string >& boneTable,
			std::vector<ControlPointInfluenceExt>& controlPointInfluences);
		
		/// <summary>
		/// Process each FBXMesh's SKIN, and store rigging data in mesh structure
		/// </summary>
		/// <param name="_poSourceFbxMesh"></param>
		/// <param name="destPackedMesh"></param>
		/// <returns></returns>
		static bool ProcessSkin(
			FbxMesh* _poSourceFbxMesh,
			PackedMesh& destPackedMesh);

	private:
		static bool GetInfluencesFromSkin(
			fbxsdk::FbxSkin* poSkin,
			fbxsdk::FbxMesh* poFbxMeshNode,
			PackedMesh& destPackedMesh,
			const std::vector<std::string>& boneTable,
			std::vector<ControlPointInfluenceExt>& controlPointInfluences);
	};

}
