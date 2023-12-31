using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGLTF;


	public static class ExportGlb
	{
		public static event Action<string> OnResultFilePathAssigned;

		public static string ResultFilePath;
		
		private const string MenuPrefix = "Assets/UnityGLTF/";

	    public static string RetrieveTexturePath(UnityEngine.Texture texture)
	    {
	        var path = AssetDatabase.GetAssetPath(texture);
	        // texture is a subasset
	        if (AssetDatabase.GetMainAssetTypeAtPath(path) != typeof(Texture2D))
	        {
		        var ext = System.IO.Path.GetExtension(path);
		        if (string.IsNullOrWhiteSpace(ext)) return texture.name + ".png";
		        path = path.Replace(ext, "-" + texture.name + ext);
	        }
	        return path;
	    }

	    private static bool TryGetExportNameAndRootTransformsFromSelection(out string sceneName, out Transform[] rootTransforms)
	    {
		    if (Selection.transforms.Length > 1)
		    {
			    sceneName = SceneManager.GetActiveScene().name;
			    rootTransforms = Selection.transforms;
			    return true;
		    }
		    if (Selection.transforms.Length == 1)
		    {
			    sceneName = Selection.activeGameObject.name;
			    rootTransforms = Selection.transforms;
			    return true;
		    }
		    if (Selection.objects.Any() && Selection.objects.All(x => x is GameObject))
		    {
			    sceneName = Selection.objects.First().name;
			    rootTransforms = Selection.objects.Select(x => (x as GameObject).transform).ToArray();
			    return true;
		    }

		    sceneName = null;
		    rootTransforms = null;
		    return false;
	    }
	    

	    
		public static void ExportGLBByName(string gameObjectName)
		{
			GameObject targetGameObject = GameObject.Find(gameObjectName);
			if (targetGameObject == null)
			{
				Debug.LogError("Can't export: GameObject not found");
				return;
			}

			string sceneName = targetGameObject.name;
			Transform[] rootTransforms = new Transform[] { targetGameObject.transform };

			Export(rootTransforms, true, sceneName);
		}
		

		private static void Export(Transform[] transforms, bool binary, string sceneName)
		{
			var settings = GLTFSettings.GetOrCreateSettings();
			var exportOptions = new ExportOptions { TexturePathRetriever = RetrieveTexturePath };
			var exporter = new GLTFSceneExporter(transforms, exportOptions);

			/*
			var invokedByShortcut = Event.current?.type == EventType.KeyDown;
			var path = settings.SaveFolderPath;
			if (!invokedByShortcut || !Directory.Exists(path))
				path = EditorUtility.SaveFolderPanel("glTF Export Path", settings.SaveFolderPath, "");
				*/
			
			var path = Path.Combine(Application.dataPath, "glTFExports");


			if (!string.IsNullOrEmpty(path))
			{
				var ext = binary ? ".glb" : ".gltf";
				var resultFile = GLTFSceneExporter.GetFileName(path, sceneName, ext);
				settings.SaveFolderPath = path;
				if(binary)
					exporter.SaveGLB(path, sceneName);
				else
					exporter.SaveGLTFandBin(path, sceneName);

				Debug.Log("Exported to " + resultFile);
				//Open the folder in the file browser
				//EditorUtility.RevealInFinder(resultFile);
				ResultFilePath = resultFile;
				
				OnResultFilePathAssigned?.Invoke(ResultFilePath);


			}
		}
	}