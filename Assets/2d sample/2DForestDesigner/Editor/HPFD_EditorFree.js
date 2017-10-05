#pragma strict
import System.IO;
import UnityEditor.SceneManagement;

class ForestDesigner extends EditorWindow {

	private var daCurrLayerObj			: GameObject;
	private var daMainObject			: GameObject;
	private var daSubParent				: GameObject;
	private var HPFDparent				: GameObject;

	private var tempTransform 			: Transform;

	private var mainPath 				: String;
	private var tempString 				: String;
	private var tempStringTwo			: String;
	private var tempStringThree			: String;
	private var theCreationFileName 	: String;
	private var HPFDpath 				: String;

	private var randomY 				: float;
	private var randomRotate			: float;
	private var PosX 					: float = 0;
	private var PosY 					: float = 0;
	private var PosZ 					: float = 0;
	private var tempFloatOne			: float;
	private var tempFloatTwo			: float;
	private var currSibling 			: float = 0;
	private var tempX 					: float;
	private var tempY 					: float;
	private var tempX2 					: float;
	private var tempY2 					: float;
	private var daSubParentScale		: float;
	private var HPFDglobalScale			: float;

	private var boolOne					: boolean = false;
	private var boolTwo					: boolean = false;
	private var runOnceBool				: boolean = true;
	private var assetCheckBool			: boolean = false;
	private var assetCheckBool2			: boolean = false;
	private var autoSelectBool 			: boolean;
	private var displayGUI 				: boolean = false;
	private var directoryFind 			: boolean = true;

	private var autoSelectGameObject	: int = 1;
	private var wizardPage 				: int = 0;
	private var tempInt 				: int = 0;
	private var defaultQualityInt 		: int = 0;
	private var numOfTreeRoots 			: int = 5+1;
	private var numOfTreeTop			: int = 3+1;
	private var numOfTreeTrunk			: int = 1+1;
	private var numOfTreeLeavesBulk		: int = 3+1;
	private var numOfPlants				: int = 5+1;
	private var numOfRocks 				: int = 4+1;

	private var texture1       			: Texture2D;
	private var texture2       			: Texture2D;
	private var buttonBack				: Texture2D;

	private var scrollPosition  		: Vector2;

	private var daPos 					: Vector3;

	@MenuItem ("Window/2D Forest Designer")
	static function ShowWindow () {
		EditorWindow.GetWindow (ForestDesigner);
	}

	static function Init() {
		var window = ScriptableObject.CreateInstance.<ForestDesigner>();
		window.Show();
	}

	function OnGUI () {
		if ( HPFDparent == null || daSubParent == null ){
			runOnceBool = true;
			wizardPage 	= 0;
		}
		if (directoryFind){
			var guids: String[] = AssetDatabase.FindAssets("HPFD_EditorFree");
			for (var guid: String in guids) {
				var HPFDpathTemp = AssetDatabase.GUIDToAssetPath(guid);
				HPFDpath = HPFDpathTemp.Substring(0,(HPFDpathTemp.length - 26));
			}
			directoryFind = false;
		}
		switch(wizardPage){
			case 0:
				if (runOnceBool){
					runOnceBool 	= false;
					boolTwo			= false;
					daMainObject 	= null;
					daCurrLayerObj 	= null;
					currSibling		= 0;
					if ( PlayerPrefs.GetInt("HPFDdefaultQualityInt") > 0){
						defaultQualityInt = PlayerPrefs.GetInt("HPFDdefaultQualityInt");
					} else {
						PlayerPrefs.SetInt("HPFDdefaultQualityInt", defaultQualityInt);
					}
					
					if ( PlayerPrefs.GetInt("HPFDautoSelectGameObject") > 0){
						autoSelectGameObject = PlayerPrefs.GetInt("HPFDautoSelectGameObject");
					} else {
						PlayerPrefs.SetInt("HPFDautoSelectGameObject", autoSelectGameObject);
					}

					if (autoSelectGameObject == 2){
						autoSelectBool = false;
					} else {
						autoSelectBool = true;
					}

					buttonBack = AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_Back.psd", Texture2D) as Texture2D;
					if (GameObject.Find("ForestDesigner")){
						HPFDparent = GameObject.Find("ForestDesigner");
					} else {
						HPFDparent 	= new GameObject ("ForestDesigner");
						daSubParent = new GameObject ("TREES");
						daSubParent.transform.SetParent(HPFDparent.transform, false);
						daSubParent = new GameObject ("PLANTS");
						daSubParent.transform.SetParent(HPFDparent.transform, false);
						daSubParent = new GameObject ("ROCKS");
						daSubParent.transform.SetParent(HPFDparent.transform, false);
					}
				}

				scrollPosition = GUI.BeginScrollView (Rect (10,0,(Screen.width-10),(Screen.height-10)),
													scrollPosition, Rect (0, 0, 120, 520));
				GUILayout.BeginArea (Rect (10,20,300,500));
				texture1 = AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_Trees.psd", Texture2D) as Texture2D;
				if(GUILayout.Button(texture1, GUILayout.Width(200), GUILayout.Height(110))) {
					runOnceBool = true;
					if (GameObject.Find("/ForestDesigner/TREES")){
						daSubParent = GameObject.Find("/ForestDesigner/TREES");
					} else {
						daSubParent = new GameObject ("TREES");
						daSubParent.transform.SetParent(HPFDparent.transform, false);
					}
					if (daSubParent.transform.childCount > 0){
						for (var i = 0; i < daSubParent.transform.childCount; i++) {
							if (daSubParent.transform.GetChild(i).name.Substring(5,4) == "Tree"){
								daMainObject = daSubParent.transform.GetChild(i).gameObject;
								RefreshPosition(daMainObject);
								break;
							}
						}
					}
					daSubParentScale = daSubParent.transform.localScale.x;
					wizardPage = 1;
				}
				GUILayout.EndArea ();

				GUILayout.BeginArea (Rect (10,140,300,500));
				texture1 = AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_Plants.psd", Texture2D) as Texture2D;
				if(GUILayout.Button(texture1, GUILayout.Width(200), GUILayout.Height(110))) {
					runOnceBool = true;
					if (GameObject.Find("/ForestDesigner/PLANTS")){
						daSubParent = GameObject.Find("/ForestDesigner/PLANTS");
					} else {
						daSubParent = new GameObject ("PLANTS");
						daSubParent.transform.SetParent(HPFDparent.transform, false);
					}
					if (daSubParent.transform.childCount > 0){
						for ( i = 0; i < daSubParent.transform.childCount; i++) {
							if (daSubParent.transform.GetChild(i).name.Substring(5,5) == "Plant"){
								daMainObject = daSubParent.transform.GetChild(i).gameObject;
								RefreshPosition(daMainObject);
								break;
							}
						}
					}
					daSubParentScale = daSubParent.transform.localScale.x;
					wizardPage = 2;
				}
				GUILayout.EndArea ();

				GUILayout.BeginArea (Rect (10,260,300,500));
				texture1 = AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_Rocks.psd", Texture2D) as Texture2D;
				if(GUILayout.Button(texture1, GUILayout.Width(200), GUILayout.Height(110))) {
					runOnceBool = true;
					if (GameObject.Find("/ForestDesigner/ROCKS")){
						daSubParent = GameObject.Find("/ForestDesigner/ROCKS");
					} else {
						daSubParent = new GameObject ("ROCKS");
						daSubParent.transform.SetParent(HPFDparent.transform, false);
					}
					if (daSubParent.transform.childCount > 0){
						for ( i = 0; i < daSubParent.transform.childCount; i++) {
							if (daSubParent.transform.GetChild(i).name.Substring(5,5) == "Rocks"){
								daMainObject = daSubParent.transform.GetChild(i).gameObject;
								RefreshPosition(daMainObject);
								break;
							}
						}
					}
					daSubParentScale = daSubParent.transform.localScale.x;
					wizardPage = 3;
				}
				GUILayout.EndArea ();

				GUILayout.BeginArea (Rect (10,380,300,500));
				texture1 = AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_Settings.psd", Texture2D) as Texture2D;
				if(GUILayout.Button(texture1, GUILayout.Width(200), GUILayout.Height(110))) {
					runOnceBool = true;
					
					HPFDglobalScale = HPFDparent.transform.localScale.x;
					wizardPage = 4;
				}
				GUILayout.EndArea ();

				GUI.EndScrollView ();
			break;


			case 1:

				scrollPosition = GUI.BeginScrollView (Rect (10,0,(Screen.width-10),(Screen.height-80)),
													scrollPosition, Rect (0, 0, 120, 550));
				GUILayout.BeginArea (Rect (10,10,250,500));
				EditorGUILayout.LabelField( "Buy Full version from the asset store" );
				EditorGUILayout.LabelField( "for more designs and features" );
				GUILayout.EndArea ();

				GUILayout.BeginArea (Rect (10,50,300,500));
				if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_GenTree.psd", Texture2D) as Texture2D
									, GUILayout.Width(200), GUILayout.Height(40))) {
					
					GenerateButtonPress("Trees", -10, -10);

						GenerateObjectName( "treeRoots", true, true, "N", "H", 1, "01", "BasicRoots" );
						CreateElement ( theCreationFileName, mainPath, 0, 0, 0, 1, 1, 1 );

					if (Random.value > 0.8){
						GenerateObjectName( "treeTop", true, true, "N", "H", 1, "01", "01" );
						CreateElement ( theCreationFileName, mainPath, 0, 10.235, 0, 1, 1, 1 );
					} else {
						GenerateObjectName( "treeTop", true, true, "N", "H", 1, "01", "01" );
						CreateElement ( theCreationFileName, mainPath, 0, 10.235, 1, 1, 1, 1 );
						tempString = theCreationFileName.Substring(8,2);
						for ( i = 1; i < 10; i++) {
							tempStringTwo = "0" + i.ToString();
									GenerateObjectName( "treeLeavesBulk", true, true, "S", "H", 1, tempString, tempStringTwo );
							
							FindIfAssetIsAvailable(theCreationFileName, mainPath);
							if (assetCheckBool && Random.value > 0.2){
								CreateElement ( theCreationFileName, mainPath, 0, 10.2, ( 0 - i ), 1, 1, 1 );
							}
						}
					}
					RefreshPosition(daMainObject);
				}
				GUILayout.EndArea ();

				if (daMainObject != null){
					GUILayout.BeginArea (Rect (10,100,300,500));
					GUILayout.BeginHorizontal ();

					if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/PosButtons/PosButton12.png", Texture2D) as Texture2D
											, GUILayout.Width(30), GUILayout.Height(30))) {
						SelectPrevious("Trees");

					}
					if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/PosButtons/PosButton14.png", Texture2D) as Texture2D
											, GUILayout.Width(30), GUILayout.Height(30))) {
						SelectNext("Trees");
					}
					
					EditorGUILayout.LabelField( daMainObject.transform.name );
					GUILayout.EndHorizontal ();
					GUILayout.EndArea ();

					if ( daMainObject.transform.childCount > 0 ){
						GUILayout.BeginArea (Rect (10,140,300,500));
						if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_RandomTree.psd", Texture2D) as Texture2D
											, GUILayout.Width(200), GUILayout.Height(40))) {
							RefreshPosition(daMainObject);
							PosX = daMainObject.transform.localPosition.x;
							PosY = daMainObject.transform.localPosition.y;
							PosZ = daMainObject.transform.localPosition.z;
							tempString = daMainObject.transform.name;
							DestroyImmediate (daMainObject);
							daMainObject = new GameObject (tempString);
							daMainObject.transform.SetParent(daSubParent.transform, false);

							daMainObject.transform.localPosition.x = PosX;
							daMainObject.transform.localPosition.y = PosY;
							daMainObject.transform.localPosition.z = PosZ;

								GenerateObjectName( "treeRoots", true, true, "N", "H", 1, "01", "BasicRoots" );
								CreateElement ( theCreationFileName, mainPath, 0, 0, 0, 1, 1, 1 );

							if (Random.value > 0.5){
								GenerateObjectName( "treeTop", true, true, "N", "H", 1, "01", "01" );
								CreateElement ( theCreationFileName, mainPath, 0, 10.235, 0, 1, 1, 1 );
							} else {
								GenerateObjectName( "treeTop", true, true, "N", "H", 1, "01", "01" );
								CreateElement ( theCreationFileName, mainPath, 0, 10.235, 1, 1, 1, 1 );
								tempString = theCreationFileName.Substring(8,2);

								for ( i = 1; i < 10; i++) {
									tempStringTwo = "0" + i.ToString();
									GenerateObjectName( "treeLeavesBulk", true, true, "S", "H", 1, tempString, tempStringTwo );
									
									FindIfAssetIsAvailable(theCreationFileName, mainPath);
									if (assetCheckBool && Random.value > 0.2){
										CreateElement ( theCreationFileName, mainPath, 0, 10.2, ( 0 - i ), 1, 1, 1 );
									}
								}								
							}
						}
						GUILayout.EndArea ();

						GUILayout.BeginArea (Rect (10,190,300,500));
						if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_RandomLeaves.psd", Texture2D) as Texture2D
											, GUILayout.Width(200), GUILayout.Height(40))) {
							if (daMainObject != null){
								PosX = daMainObject.transform.localPosition.x;
								PosY = daMainObject.transform.localPosition.y;
								PosZ = daMainObject.transform.localPosition.z;

								if (daMainObject.transform.childCount>0){
									for ( i = 0; i < daMainObject.transform.childCount ; i++ ) {
										tempString = daMainObject.transform.GetChild(i).name.Substring(12,2);
										if (tempString == "BG"){
											DestroyImmediate ( daMainObject.transform.GetChild(i).gameObject );
											i = 0;
										}
									}
								}

								for ( i = 0; i < daMainObject.transform.childCount; i++) {
									if ( daMainObject.transform.GetChild(i).GetComponent.<SpriteRenderer>()&&
											 daMainObject.transform.GetChild(i).GetComponent.<SpriteRenderer>().sprite.name.Substring(5,3) == "TTP"){
										tempString = daMainObject.transform.GetChild(i).GetComponent.<SpriteRenderer>().sprite.name.Substring(8,2);
										break;
									}
								}
								for ( i = 1; i < 10; i++) {
									tempStringTwo = "0" + i.ToString();
									GenerateObjectName( "treeLeavesBulk", true, true, "S", "H", 1, tempString, tempStringTwo );
									
									FindIfAssetIsAvailable(theCreationFileName, mainPath);
									if (assetCheckBool && Random.value > 0.2){
										CreateElement ( theCreationFileName, mainPath, 0, 10.2, ( 0 - i ), 1, 1, 1 );
									}
								}
							}
							RefreshPosition(daMainObject);
						}
						GUILayout.EndArea ();

						GUILayout.BeginArea (Rect (10,240,300,500));
						if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_RandomRoots.psd", Texture2D) as Texture2D
											, GUILayout.Width(200), GUILayout.Height(40))) {
							if (daMainObject != null){
								PosX = daMainObject.transform.localPosition.x;
								PosY = daMainObject.transform.localPosition.y;
								PosZ = daMainObject.transform.localPosition.z;

								if (daMainObject.transform.childCount>0){
									for ( i = 0; i < daMainObject.transform.childCount ; i++ ) {
										tempString = daMainObject.transform.GetChild(i).name.Substring(5,3);
										if (tempString == "TRT"){
											DestroyImmediate ( daMainObject.transform.GetChild(i).gameObject );
											i = 0;
										}
									}
								}
								GenerateObjectName( "treeRoots", true, true, "N", "H", 1, "01", "BasicRoots" );
								CreateElement ( theCreationFileName, mainPath, 0, 0, 0, 1, 1, 1 );
							}
							RefreshPosition(daMainObject);
						}

						GUILayout.EndArea ();

						GUILayout.BeginArea (Rect (10,310,300,500));
						EditorGUILayout.LabelField( "Saving to texture may take time" );
						if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_SaveTexture.psd", Texture2D) as Texture2D
										, GUILayout.Width(200), GUILayout.Height(40))) {
							assetCheckBool = false;
							if ( daMainObject.transform.name.Length > 20 ){
								FindIfAssetIsAvailable(daMainObject.transform.name.Substring(0,21), HPFDpath + "/Prefab");
							}
							if (assetCheckBool){
								DeleteAssetsWithName( daMainObject.transform.name.Substring(0,21), HPFDpath + "/SavedImages" );
								SaveTextureToFile(( daMainObject.transform.name.Substring(0,21)), 2048, 2048);
							} else {
								SaveTextureToFile(( daMainObject.transform.name.Substring(0,11) + System.DateTime.UtcNow.ToString("yyddHHmmss")), 2048, 2048);
							}
							RefreshPosition(daMainObject);
						}
						GUILayout.EndArea ();

						GUILayout.BeginArea (Rect (10,390,200,500));
						EditorGUILayout.LabelField( "Trees Gobal Scale:" );
						daSubParentScale = EditorGUILayout.Slider(daSubParentScale, 0.01, 1);
						daSubParent.transform.localScale = Vector3(daSubParentScale, daSubParentScale, 1);
						GUILayout.EndArea ();

					} else {
						AfterSaveMenu();
					}
				} else {
					if (daSubParent.transform.childCount > 0){
						for (i = 0; i < daSubParent.transform.childCount; i++) {
							if ( daSubParent.transform.GetChild(i).name.Substring(5,4) == "Tree" ){
								daMainObject = daSubParent.transform.GetChild(i).gameObject;
								break;
							}
						}
					}
				}
				GUI.EndScrollView ();

				GUILayout.BeginArea (Rect (20,(Screen.height-70),300,500));
				if(GUILayout.Button(buttonBack, GUILayout.Width(145), GUILayout.Height(45))) {
					runOnceBool = true;
					wizardPage = 0;
				}
				GUILayout.EndArea ();

			break;

			case 2:

				scrollPosition = GUI.BeginScrollView (Rect (10,0,(Screen.width-10),(Screen.height-80)),
													scrollPosition, Rect (0, 0, 120, 550));

				GUILayout.BeginArea (Rect (10,10,250,500));
				EditorGUILayout.LabelField( "Buy Full version from the asset store" );
				EditorGUILayout.LabelField( "for more designs and features" );
				GUILayout.EndArea ();

				GUILayout.BeginArea (Rect (10,50,300,500));
				if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_GenPlant.psd", Texture2D) as Texture2D
									, GUILayout.Width(200), GUILayout.Height(40))) {

					GenerateButtonPress("Plant", -8, -3);

					var daInt1 = Random.Range(1, 5);
					for (i = 0; i < daInt1; i++) {
						GenerateObjectName( "plants", true, true, "S", "H", 1, tempString, "01" );

						if (i == 0){
							CreateElement ( theCreationFileName, mainPath, 0, 0, 0, 1, 1, 1 );							
						} else {
							CreateElement ( theCreationFileName, mainPath, Random.Range(0.0, 1.0), 0, 0, Random.Range(0.3, 1), Random.Range(0.3, 1), 1 );
						}
					}
					RefreshPosition(daMainObject);

				}
				GUILayout.EndArea ();

				if (daMainObject){
					GUILayout.BeginArea (Rect (10,100,300,500));
					GUILayout.BeginHorizontal ();

					if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/PosButtons/PosButton12.png", Texture2D) as Texture2D
											, GUILayout.Width(30), GUILayout.Height(30))) {
						
						SelectPrevious("Plant");
					}
					if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/PosButtons/PosButton14.png", Texture2D) as Texture2D
											, GUILayout.Width(30), GUILayout.Height(30))) {
						
						SelectNext("Plant");
					}

					EditorGUILayout.LabelField( daMainObject.transform.name );

					GUILayout.EndHorizontal ();
					GUILayout.EndArea ();
					if (daMainObject.transform.childCount > 0){
						GUILayout.BeginArea (Rect (10,150,300,500));
						if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_RandomPlant.psd", Texture2D) as Texture2D
											, GUILayout.Width(200), GUILayout.Height(40))) {
							PosX = daMainObject.transform.localPosition.x;
							PosY = daMainObject.transform.localPosition.y;
							PosZ = daMainObject.transform.localPosition.z;
							tempString = daMainObject.transform.name;
							DestroyImmediate (daMainObject);
							
							daMainObject = new GameObject (tempString);

							daMainObject.transform.SetParent(daSubParent.transform, false);

							daMainObject.transform.localPosition.x = PosX;
							daMainObject.transform.localPosition.y = PosY;
							daMainObject.transform.localPosition.z = PosZ;
							daInt1 = Random.Range(1, 5);
							for (i = 0; i < daInt1; i++) {
								GenerateObjectName( "plants", true, true, "S", "H", 1, "01", "01" );
								
								if (i == 0){
									CreateElement ( theCreationFileName, mainPath, 0, 0, 0, 1, 1, 1 );							
								} else {
									CreateElement ( theCreationFileName, mainPath, Random.Range(0.0, 1.0), 0, 0, Random.Range(0.3, 1), Random.Range(0.3, 1), 1 );
								}
							}
							RefreshPosition(daMainObject);
						}
						GUILayout.EndArea ();

						GUILayout.BeginArea (Rect (10,220,300,500));
						EditorGUILayout.LabelField( "Saving to texture may take time" );
						if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_SaveTexture.psd", Texture2D) as Texture2D
										, GUILayout.Width(200), GUILayout.Height(40))) {
							assetCheckBool = false;
							if ( daMainObject.transform.name.Length > 20 ){
								FindIfAssetIsAvailable(daMainObject.transform.name.Substring(0,21), HPFDpath + "/Prefab");
							}
							if (assetCheckBool){
								DeleteAssetsWithName( daMainObject.transform.name.Substring(0,21), HPFDpath + "/SavedImages" );
								SaveTextureToFile(( daMainObject.transform.name.Substring(0,21)), 1024, 1024);
							} else {
								SaveTextureToFile(( daMainObject.transform.name.Substring(0,11) + System.DateTime.UtcNow.ToString("yyddHHmmss")), 1024, 1024);
							}
							RefreshPosition(daMainObject);
						}

						GUILayout.EndArea ();

						GUILayout.BeginArea (Rect (10,300,200,500));
						EditorGUILayout.LabelField( "Plants Gobal Scale:" );
						daSubParentScale = EditorGUILayout.Slider(daSubParentScale, 0.01, 1);
						daSubParent.transform.localScale = Vector3(daSubParentScale, daSubParentScale, 1);
						GUILayout.EndArea ();
					} else {
						AfterSaveMenu();
					}

				} else {
					if (daSubParent.transform.childCount > 0){
						for (i = 0; i < daSubParent.transform.childCount; i++) {
							if ( daSubParent.transform.GetChild(i).name.Substring(5,5) == "Plant" ){
								daMainObject = daSubParent.transform.GetChild(i).gameObject;
								break;
							}
						}
					}
				}

				GUI.EndScrollView ();

				GUILayout.BeginArea (Rect (20,(Screen.height-70),300,500));
				if(GUILayout.Button(buttonBack, GUILayout.Width(145), GUILayout.Height(45))) {
					runOnceBool = true;
					wizardPage = 0;
				}
				GUILayout.EndArea ();
			break;

			case 3:

				scrollPosition = GUI.BeginScrollView (Rect (10,0,(Screen.width-10),(Screen.height-80)),
													scrollPosition, Rect (0, 0, 120, 550));

				GUILayout.BeginArea (Rect (10,10,300,500));
				EditorGUILayout.LabelField( "Buy Full version from the asset store" );
				EditorGUILayout.LabelField( "for more designs and features" );
				GUILayout.EndArea ();

				GUILayout.BeginArea (Rect (10,50,300,500));
				if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_GenRocks.psd", Texture2D) as Texture2D
									, GUILayout.Width(200), GUILayout.Height(40))) {

					GenerateButtonPress("Rocks", -8, -3);

					daInt1 = Random.Range(1, 5);
					if (Random.value > 0.5){
						tempStringTwo = "01";
					} else {
						tempStringTwo = "02";
					}
					for (i = 0; i < daInt1; i++) {
						GenerateObjectName( "rocks", true, true, "N", "H", 1, tempStringTwo, "01" );
						if (i == 0){
							CreateElement ( theCreationFileName, mainPath, 0, 0, 0, Random.Range(0.5, 1.0), Random.Range(0.5, 1.0),1 );
						} else {
							CreateElement ( theCreationFileName, mainPath, Random.Range(0.0, 3.0), Random.value, i, Random.Range(0.5, 1.0), Random.Range(0.5, 1.0),1 );
						}

						daCurrLayerObj.transform.SetSiblingIndex(0);
					}
					RefreshPosition(daMainObject);
				}
				GUILayout.EndArea ();

				if (daMainObject){
					GUILayout.BeginArea (Rect (10,100,300,500));
					GUILayout.BeginHorizontal ();

					if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/PosButtons/PosButton12.png", Texture2D) as Texture2D
											, GUILayout.Width(30), GUILayout.Height(30))) {
						SelectPrevious("Rocks");
					}
					if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/PosButtons/PosButton14.png", Texture2D) as Texture2D
											, GUILayout.Width(30), GUILayout.Height(30))) {
						SelectNext("Rocks");
					}

					EditorGUILayout.LabelField( daMainObject.transform.name );

					GUILayout.EndHorizontal ();
					GUILayout.EndArea ();
					if (daMainObject.transform.childCount > 0){
						GUILayout.BeginArea (Rect (10,150,300,500));
						if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_RandomRocks.psd", Texture2D) as Texture2D
											, GUILayout.Width(200), GUILayout.Height(40))) {
							PosX = daMainObject.transform.localPosition.x;
							PosY = daMainObject.transform.localPosition.y;
							PosZ = daMainObject.transform.localPosition.z;
							tempString = daMainObject.transform.name;
							DestroyImmediate (daMainObject);
							
							daMainObject = new GameObject (tempString);

							daMainObject.transform.SetParent(daSubParent.transform, false);

							daMainObject.transform.localPosition.x = PosX;
							daMainObject.transform.localPosition.y = PosY;
							daMainObject.transform.localPosition.z = PosZ;
							daInt1 = Random.Range(2, 5);
							tempFloatTwo = 0;
							if (Random.value > 0.5){
								tempStringTwo = "01";
							} else {
								tempStringTwo = "02";
							}
							for (i = 0; i <= daInt1; i++) {
								GenerateObjectName( "rocks", true, true, "N", "H", 1, tempStringTwo, "01" );
								if (i == 0){
									CreateElement ( theCreationFileName, mainPath, 0, 0, 0, Random.Range(0.5, 1), Random.Range(0.5, 1),1 );
								} else {
									CreateElement ( theCreationFileName, mainPath, Random.Range(0.0, 3.0), Random.value, i, Random.Range(0.5, 1), Random.Range(0.5, 1),1 );
								}								
								daCurrLayerObj.transform.SetSiblingIndex(0);
							}
							RefreshPosition(daMainObject);
						}
						GUILayout.EndArea ();

						GUILayout.BeginArea (Rect (10,220,300,500));
						EditorGUILayout.LabelField( "Saving to texture may take time" );
						if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_SaveTexture.psd", Texture2D) as Texture2D
										, GUILayout.Width(200), GUILayout.Height(40))) {
							assetCheckBool = false;
							if ( daMainObject.transform.name.Length > 20 ){
								FindIfAssetIsAvailable(daMainObject.transform.name.Substring(0,21), HPFDpath + "/Prefab");
							}
							if (assetCheckBool){
								DeleteAssetsWithName( daMainObject.transform.name.Substring(0,21), HPFDpath + "/SavedImages" );
								SaveTextureToFile(( daMainObject.transform.name.Substring(0,21)), 2048, 2048);
							} else {
								SaveTextureToFile(( daMainObject.transform.name.Substring(0,11) + System.DateTime.UtcNow.ToString("yyddHHmmss")), 2048, 2048);
							}

							RefreshPosition(daMainObject);
						}
						GUILayout.EndArea ();

						GUILayout.BeginArea (Rect (10,300,200,500));
						EditorGUILayout.LabelField( "Rocks Gobal Scale:" );
						daSubParentScale = EditorGUILayout.Slider(daSubParentScale, 0.01, 1);
						daSubParent.transform.localScale = Vector3(daSubParentScale, daSubParentScale, 1);
						GUILayout.EndArea ();

					} else {
						AfterSaveMenu();
					}

				} else {
					if (daSubParent.transform.childCount > 0){
						for (i = 0; i < daSubParent.transform.childCount; i++) {
							if ( daSubParent.transform.GetChild(i).name.Substring(5,5) == "Plant" ){
								daMainObject = daSubParent.transform.GetChild(i).gameObject;
								break;
							}
						}
					}
				}

				GUI.EndScrollView ();

				GUILayout.BeginArea (Rect (20,(Screen.height-70),300,500));
				if(GUILayout.Button(buttonBack, GUILayout.Width(145), GUILayout.Height(45))) {
					runOnceBool = true;
					wizardPage = 0;
				}
				GUILayout.EndArea ();
			break;

			case 4:
				scrollPosition = GUI.BeginScrollView (Rect (10,0,(Screen.width-10),(Screen.height-80)),
													scrollPosition, Rect (0, 0, 120, 300));

				GUILayout.BeginArea (Rect (10,30,200,500));
				EditorGUILayout.LabelField( "Gobal Scale:" );
				HPFDglobalScale = EditorGUILayout.Slider(HPFDglobalScale, 0.01, 1);
				HPFDparent.transform.localScale = Vector3(HPFDglobalScale, HPFDglobalScale, 1);
				GUILayout.EndArea ();

				GUILayout.BeginArea (Rect (10,130,300,500));
				autoSelectBool = EditorGUI.Toggle(Rect(0,5,position.width,20),
						"Auto Select Game Object",
						autoSelectBool);
				GUILayout.EndArea ();


				GUILayout.BeginArea (Rect (10,180,300,500));
				if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_SaveAllTextures.psd", Texture2D) as Texture2D
									, GUILayout.Width(200), GUILayout.Height(40))) {
					PlayerPrefs.SetInt("defaultQualityInt", defaultQualityInt);
					wizardPage = 5;
				}
				GUILayout.EndArea ();

				GUI.EndScrollView ();

				GUILayout.BeginArea (Rect (20,(Screen.height-70),300,500));
				if(GUILayout.Button(buttonBack, GUILayout.Width(145), GUILayout.Height(45))) {
					if (autoSelectBool){
						PlayerPrefs.SetInt("autoSelectGameObject", 1);
						autoSelectGameObject = 1;
					} else {
						PlayerPrefs.SetInt("autoSelectGameObject", 2);
						autoSelectGameObject = 2;
					}
					runOnceBool = true;
					PlayerPrefs.SetInt("defaultQualityInt", defaultQualityInt);
					wizardPage = 0;
				}
				GUILayout.EndArea ();
			break;

			case 5:
				scrollPosition = GUI.BeginScrollView (Rect (10,0,(Screen.width-10),(Screen.height-80)),
													scrollPosition, Rect (0, 0, 120, 300));
				
				EditorGUI.DrawPreviewTexture(Rect(10,30,200,200),AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_SaveAllWarning.psd", Texture2D) as Texture2D);

				GUILayout.BeginArea (Rect (10,250,300,500));
				if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_SaveAllTextures.psd", Texture2D) as Texture2D
									, GUILayout.Width(200), GUILayout.Height(40))) {
					for ( i = 0; i < HPFDparent.transform.childCount; i++ ) {
						daSubParent = HPFDparent.transform.GetChild(i).gameObject;
						for (var k = 0; k < daSubParent.transform.childCount; k++) {
							daMainObject = daSubParent.transform.GetChild(k).gameObject;
							if (daMainObject.transform.childCount > 0 ){
								assetCheckBool = false;
								if ( daMainObject.transform.name.Length > 20 ){
									FindIfAssetIsAvailable(daMainObject.transform.name.Substring(0,21), HPFDpath + "/Prefab");
								}
								if (assetCheckBool){
									DeleteAssetsWithName( daMainObject.transform.name.Substring(0,21), HPFDpath + "/SavedImages" );
									SaveTextureToFile(( daMainObject.transform.name.Substring(0,21)), 2048, 2048);
								} else {
									SaveTextureToFile(( daMainObject.transform.name.Substring(0,11) + System.DateTime.UtcNow.ToString("yyddHHmmss")), 2048, 2048);
								}
							}
							if (daMainObject.transform.childCount>0){
								k = k - 1;
							}
						}
					}
					wizardPage = 4;
				}
				GUILayout.EndArea ();

				GUI.EndScrollView ();

				GUILayout.BeginArea (Rect (20,(Screen.height-70),300,500));
				if(GUILayout.Button(buttonBack, GUILayout.Width(145), GUILayout.Height(45))) {
					runOnceBool = true;
					wizardPage = 4;
				}
				GUILayout.EndArea ();
			break;
		}
	}

	function CreateElement(daImageName : String, daAssetPath : String, daPosX : float, daPosY : float, daPosZ : float, daScaleX : float, daScaleY : float, daScaleZ : float){
		var databaseFind = AssetDatabase.FindAssets (daImageName, [daAssetPath]);
		for (var guid in databaseFind){
			var textureViewer = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), Texture2D) as Texture2D;
			var createPath = AssetDatabase.GUIDToAssetPath(guid);

			daCurrLayerObj = new GameObject (daImageName);

			daCurrLayerObj.transform.localPosition.x = daPosX;
			daCurrLayerObj.transform.localPosition.y = daPosY;
			daCurrLayerObj.transform.localPosition.z = daPosZ;

			daCurrLayerObj.transform.localScale.x = daScaleX;
			daCurrLayerObj.transform.localScale.y = daScaleY;
			daCurrLayerObj.transform.localScale.z = daScaleZ;

			daCurrLayerObj.AddComponent.<SpriteRenderer>();
			daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath(createPath, Sprite) as Sprite;
			daCurrLayerObj.transform.SetParent(daMainObject.transform, false);
		}
		EditorSceneManager.MarkAllScenesDirty();
	}

	function FindIfAssetIsAvailable(daAssetName : String, daAssetPath : String){
		tempStringThree = "nil";
		assetCheckBool = false;
		var databaseFind = AssetDatabase.FindAssets (daAssetName, [daAssetPath]);
		for (var guid in databaseFind){
			tempStringThree = AssetDatabase.GUIDToAssetPath(guid);
			if (tempStringThree != "nil"){
				assetCheckBool = true;
			}
		}
	}
	function FindIfAssetIsAvailable2(daAssetName : String, daAssetPath : String){
		tempStringThree = "nil";
		assetCheckBool2 = false;
		var databaseFind = AssetDatabase.FindAssets (daAssetName, [daAssetPath]);
		for (var guid in databaseFind){
			tempStringThree = AssetDatabase.GUIDToAssetPath(guid);
			if (tempStringThree != "nil"){
				assetCheckBool2 = true;
			}
		}
	}

	function DeleteAssetsWithName(daAssetName : String, daAssetPath : String){
		tempStringThree = "nil";
		var databaseFind = AssetDatabase.FindAssets (daAssetName, [daAssetPath]);
		for (var guid in databaseFind){
			tempStringThree = AssetDatabase.GUIDToAssetPath(guid);
			if (tempStringThree != "nil"){
				AssetDatabase.DeleteAsset (AssetDatabase.GUIDToAssetPath(guid));
			}
		}
	}

	function RefreshPosition( PointObject : GameObject ){
		PosX = PointObject.transform.localPosition.x;
		PosY = PointObject.transform.localPosition.y;
		PosZ = PointObject.transform.localPosition.z;
		for (var i = 0; i < PointObject.transform.childCount; i++) {
			if (PointObject.transform.GetChild(i).name == "blankimage4test_0"){
				DestroyImmediate ( PointObject.transform.GetChild(i).gameObject );
				i--;
			}
		}
		if (autoSelectGameObject == 1){
			Selection.activeGameObject = PointObject;
		}
		boolTwo = false;
	}

	function GenerateObjectName(daObjectName : String, daRandomizer : boolean, nextDesign : boolean, theSeason : String, daQuality : String, currDesignNum : int, stringOne : String, stringTwo : String){
		var designNumberString : String;
		var daEffects = "N";

		switch (daObjectName){
			case "treeRoots":
				mainPath = ( HPFDpath + "/Modular/Trees_" + daEffects );
					if (daRandomizer){
						var designNumber = Random.Range(1, numOfTreeRoots);
					} else {
						if (nextDesign){
							currDesignNum = currDesignNum++;
							if ( currDesignNum >= numOfTreeRoots ){
								currDesignNum = 1;
							}
						} else {
							currDesignNum = currDesignNum - 1;
							if ( currDesignNum == 0 ){
								currDesignNum = numOfTreeRoots - 1;
							}
						}
					}
					if (designNumber<10){
						designNumberString = "0" + designNumber.ToString();
					} else {
						designNumberString = designNumber.ToString();
					}
					theCreationFileName = "HPFD_TRT"+designNumberString+theSeason+"_BR0101"+daQuality+daEffects+"_0";
			break;
			case "treeTop":
				mainPath = ( HPFDpath + "/Modular/Trees_" + daEffects );
				if (daRandomizer){
					designNumber = Random.Range(1, numOfTreeTop);
				} else {
					if (nextDesign){
						currDesignNum = currDesignNum++;
						if ( currDesignNum >= numOfTreeTop ){
							currDesignNum = 1;
						}
					} else {
						currDesignNum = currDesignNum - 1;
						if ( currDesignNum == 0 ){
							currDesignNum = numOfTreeTop - 1;
						}
					}
				}
				if (designNumber<10){
					designNumberString = "0" + designNumber.ToString();
				} else {
					designNumberString = designNumber.ToString();
				}
				theCreationFileName = "HPFD_TTP"+designNumberString+theSeason+"_BR0101"+daQuality+daEffects+"_0";
			break;
			case "treeLeavesBulk":
				mainPath = ( HPFDpath + "/Modular/Trees_" + daEffects );
				if (daRandomizer){
					designNumber = Random.Range(1, numOfTreeLeavesBulk);
				} else {
					if (nextDesign){
						currDesignNum = currDesignNum++;
						if ( currDesignNum >= numOfTreeLeavesBulk ){
							currDesignNum = 1;
						}
					} else {
						currDesignNum = currDesignNum - 1;
						if ( currDesignNum == 0 ){
							currDesignNum = numOfTreeLeavesBulk - 1;
						}
					}
				}
				if (designNumber<10){
					designNumberString = "0" + designNumber.ToString();
				} else {
					designNumberString = designNumber.ToString();
				}
				theCreationFileName = "HPFD_TTP"+stringOne+theSeason+"_BG"+stringTwo+designNumberString+daQuality+daEffects+"_0";
			break;
			case "treeLeavesBranch":
				
			break;
			case "rocks":
				mainPath = ( HPFDpath + "/Modular/Rocks_" + daEffects );
				if (daRandomizer){
					designNumber = Random.Range(1, numOfRocks);
				} else {
					if (nextDesign){
						currDesignNum = currDesignNum++;
						if ( currDesignNum >= numOfRocks ){
							currDesignNum = 1;
						}
					} else {
						currDesignNum = currDesignNum - 1;
						if ( currDesignNum == 0 ){
							currDesignNum = numOfRocks - 1;
						}
					}
				}
				if (designNumber<10){
					designNumberString = "0" + designNumber.ToString();
				} else {
					designNumberString = designNumber.ToString();
				}
				theCreationFileName = "HPFD_RCK"+stringOne+theSeason+"_MD"+designNumberString+"01"+daQuality+daEffects+"_0";
			break;
			case "plants":
				mainPath = ( HPFDpath + "/Modular/Plant_" + daEffects );
				if (daRandomizer){
					designNumber = Random.Range(1, numOfPlants);
				} else {
					if (nextDesign){
						currDesignNum = currDesignNum++;
						if ( currDesignNum >= numOfPlants ){
							currDesignNum = 1;
						}
					} else {
						currDesignNum = currDesignNum - 1;
						if ( currDesignNum == 0 ){
							currDesignNum = numOfPlants - 1;
						}
					}
				}
				if (designNumber<10){
					designNumberString = "0" + designNumber.ToString();
				} else {
					designNumberString = designNumber.ToString();
				}
				theCreationFileName = "HPFD_BSH01"+theSeason+"_EV"+designNumberString+"01"+daQuality+daEffects+"_0";
			break;
		}

		FindIfAssetIsAvailable2(theCreationFileName, mainPath);
		if (!assetCheckBool2){
			theCreationFileName = "blankimage4test_0";
			mainPath = HPFDpath + "/Editor/WizardImages";
		}

	}

	function SaveTextureToFile(fileName : String, daImageSizeX : float, daImageSizeY : float)
	{
		PosX = daMainObject.transform.localPosition.x;
		PosY = daMainObject.transform.localPosition.y;
		PosZ = daMainObject.transform.localPosition.z;

		if (AssetDatabase.IsValidFolder(HPFDpath + "/Prefab") == false){
			AssetDatabase.CreateFolder(HPFDpath,"Prefab");
		}

		PrefabUtility.CreatePrefab(HPFDpath + "/Prefab/" + fileName + ".prefab", daMainObject, ReplacePrefabOptions.Default);
		var daTexture = new Texture2D( daImageSizeX, daImageSizeY );
		for (var y: int = 0; y < daTexture.height; y++) {
			for (var x: int = daTexture.width; x >= 0; x--) {
				daTexture.SetPixel(x, y, Color(0,0,0,0));
			}
		}
		var childCounter = daMainObject.transform.childCount;
		if (childCounter == 0){
			childCounter = 1;
		}
		
		for (var i = 0; i < childCounter; i++) {
			if (daMainObject.transform.childCount == 0){
				tempTransform 	= daMainObject.transform;
			} else {
				tempTransform 	= daMainObject.transform.GetChild(i);
			}
			tempString 	= tempTransform.GetComponent.<SpriteRenderer>().sprite.name;
			if (tempString.Substring(0,5) == "HPFD_"){
				if (daMainObject.transform.childCount == 0){
					daCurrLayerObj 	= daMainObject;
				} else {
					daCurrLayerObj 	= daMainObject.transform.GetChild(i).gameObject;
				}
				
				if (daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.height > 0){
					var xPos = (daCurrLayerObj.transform.localPosition.x) * 100;
					var yPos = (daCurrLayerObj.transform.localPosition.y) * 100;
					var xPosMinusCounter : int = 0;
					var yPosMinusCounter : int = 0;
					while (xPos>daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.width){
						xPos = xPos - daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.width;
						xPosMinusCounter++;
					}
					while (yPos>daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.height){
						yPos = yPos - daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.height;
						yPosMinusCounter++;
					}

					var xScale = 1/daCurrLayerObj.transform.localScale.x;
					var yScale = 1/daCurrLayerObj.transform.localScale.y;

					var tempCounter = 0;
					for ( y = 0; y < daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.height; y++) {
						for ( x = 0; x < daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.width; x++ ) {
							if (x != 0 && y != 0 && x != daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.width &&
							  				y != daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.height){
								if ( daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.GetPixel(x, y).a > 0 ){
									var textColor 	= daCurrLayerObj.GetComponent.<SpriteRenderer>().sprite.texture.GetPixel(x, y);
									var mixColor 	= daCurrLayerObj.GetComponent.<SpriteRenderer>().color;

									var tempPosX = x/xScale;
									var tempPosY = y/yScale;
									Mathf.Round(tempPosX);
									Mathf.Round(tempPosY);

									var prevColor 	= daTexture.GetPixel((xPos + tempPosX) , (yPos + tempPosY));
									prevColor = prevColor * prevColor.a;
									textColor = textColor * textColor.a;
									
									var masterColor	= ( (prevColor * (1-textColor.a) + textColor) * mixColor );
									daTexture.SetPixel((xPos + tempPosX) , (yPos + tempPosY) , masterColor);
								}
							}
						}
					}
				}
			}
		}
		daTexture.Apply(false);
		var tempX : float = 0;
		var tempY : float = 0;
		for ( y = daTexture.height-1; y >=0; y-- ){
			for ( x = daTexture.width-1; x >= 0; x-- ){
				if (daTexture.GetPixel(x, y).a > 0){
					if (x > tempX){
						tempX = x;
					}
					if (y > tempY){
						tempY = y;
					}
				}
			}
		}
		var tempX2 : float  = daTexture.width;
		var tempY2 : float  = daTexture.height;
		for ( y = 0; y < daTexture.height; y++ ){
			for ( x = 0; x < daTexture.width; x++ ){
				if (daTexture.GetPixel(x,y).a > 0){
					if (x < tempX2){
						tempX2 = x;
					}
					if (y < tempY2){
						tempY2 = y;
					}
				}
			}
		}
		tempX = tempX - tempX2;
		tempY = tempY - tempY2;

		while ((tempX%4) != 0 ){
			tempX++;
		}
		while ((tempY%4) != 0 ){
			tempY++;
		}
		
		var daTexture2 = new Texture2D( tempX, tempY );
		for ( y = 0; y < daTexture2.height; y++) {
			for ( x = 0; x < daTexture2.width; x++) {
				daTexture2.SetPixel(x, y, daTexture.GetPixel(tempX2 + x, tempY2 + y));
			}
		}
		daTexture2.Apply(false);

		var bytes 			= daTexture2.EncodeToPNG();
		if (AssetDatabase.IsValidFolder(HPFDpath + "/SavedImages") == false){
			AssetDatabase.CreateFolder(HPFDpath,"SavedImages");
		}
		var path : String 	= HPFDpath + "/SavedImages/" + fileName + "_HQ" + ".png";
		var file 			= new File.Open(path,FileMode.Create);
		var binary 			= new BinaryWriter(file);
		binary.Write(bytes);
		file.Close();
		AssetDatabase.Refresh();

		var importer : TextureImporter 	= AssetImporter.GetAtPath(HPFDpath + "/SavedImages/" + fileName + "_HQ" + ".png") as TextureImporter;

		importer.wrapMode 				= TextureWrapMode.Clamp;
		importer.textureType 			= TextureImporterType.Sprite;
		importer.maxTextureSize 		= 2048;
		importer.spritePixelsPerUnit 	= 100;
		importer.filterMode 			= FilterMode.Bilinear;
		importer.isReadable 			= true;
		importer.spriteImportMode 		= SpriteImportMode.Single;
		importer.spritePivot 			= Vector2(0,0);

		AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );

		var daCurrIndex : int = daMainObject.transform.GetSiblingIndex();
		
		DestroyImmediate ( daMainObject.gameObject );

		daMainObject = new GameObject (fileName);
		daMainObject.transform.SetParent(daSubParent.transform, false);

		daMainObject.transform.SetSiblingIndex (daCurrIndex);
		tempX2 = (tempX2+(tempX/2))/100;
		tempY2 = (tempY2+(tempY/2))/100;
		daMainObject.transform.localPosition.x = tempX2 + PosX;
		daMainObject.transform.localPosition.y = tempY2 + PosY;
		daMainObject.transform.localPosition.z = PosZ;

		if (daMainObject.GetComponent.<SpriteRenderer>() == null){
			daMainObject.AddComponent.<SpriteRenderer>();
		}

		switch (defaultQualityInt){
			case 0:
				daMainObject.GetComponent.<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath(HPFDpath + "/SavedImages/" + fileName + "_HQ" + ".png", Sprite) as Sprite;
			break;
			case 1:
				LowerQualityTexture( fileName + "_HQ", fileName + "_MQ", 2 );
				daMainObject.GetComponent.<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath(HPFDpath + "/SavedImages/" + fileName + "_MQ" + ".png", Sprite) as Sprite;
			break;
			case 2:
				LowerQualityTexture( fileName + "_HQ", fileName + "_LQ", 3 );
				daMainObject.GetComponent.<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath(HPFDpath + "/SavedImages/" + fileName + "_LQ" + ".png", Sprite) as Sprite;
			break;
		}
		EditorSceneManager.MarkAllScenesDirty();
		
	}

	function LowerQualityTexture(textureName : String, saveFileName : String, qualityFloat : float)
	{
		var primaryTexture = AssetDatabase.LoadAssetAtPath(HPFDpath + "/SavedImages/" + textureName + ".png", Sprite) as Sprite;

		if (primaryTexture){
			var texture = new Texture2D((primaryTexture.texture.width/qualityFloat), (primaryTexture.texture.height/qualityFloat));
			for (var y: int = 0; y < texture.height; y++) {
				for (var x: int = texture.width; x >= 0; x--) {
					texture.SetPixel(x, y, Color(0,0,0,0));
				}
			}
			if (primaryTexture.texture.height > 0){
				for (y = 0; y < primaryTexture.texture.height; y++) {

					for ( x = 0; x < primaryTexture.texture.width; x++) {
						if ( primaryTexture.texture.GetPixel(x, y).a > 0.8 ){
							var textColor 	= primaryTexture.texture.GetPixel(x, y);
							texture.SetPixel(x/qualityFloat, y/qualityFloat, textColor);
						}
					}
				}
			}

			texture.Apply(false);

			var bytes 			= texture.EncodeToPNG();
			var path : String 	= HPFDpath + "/SavedImages/" + saveFileName + ".png";
			var file 			= new File.Open(path,FileMode.Create);
			var binary 			= new BinaryWriter(file);
			binary.Write(bytes);
			file.Close();
			AssetDatabase.Refresh();

			var importer : TextureImporter 	= AssetImporter.GetAtPath(HPFDpath + "/SavedImages/" + saveFileName + ".png") as TextureImporter;

			importer.wrapMode 				= TextureWrapMode.Clamp;
			importer.textureType 			= TextureImporterType.Sprite;
			importer.maxTextureSize 		= 2048;
			importer.spritePixelsPerUnit 	= 100/qualityFloat;
			importer.filterMode 			= FilterMode.Bilinear;
			importer.isReadable 			= true;
			importer.spriteImportMode 		= SpriteImportMode.Single;
			importer.spritePivot 			= Vector2(0,0);

			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );
		}
		EditorSceneManager.MarkAllScenesDirty();
	}

	function GenerateButtonPress(genObjName : String, nullPosX : float, nullPosY : float){
		tempInt = 1;
		for ( var i = 0; i < daSubParent.transform.childCount; i++) {
			if (daSubParent.transform.GetChild(i).name.Substring(5,5) == genObjName){
				tempInt++;
			}
		}
		daMainObject = new GameObject ("HPFD_" + genObjName + "_" + tempInt.ToString());
		daMainObject.transform.SetParent(daSubParent.transform, false);

		if(Selection.activeGameObject == null){
			RefreshPosition(daMainObject);
			PosX = nullPosX;
			PosY = nullPosY;
		} else {
			RefreshPosition(Selection.activeGameObject);
		}

		daMainObject.transform.localPosition.x = PosX + 5;
		daMainObject.transform.localPosition.y = PosY;
		daMainObject.transform.localPosition.z = PosZ;
	}

	function SelectPrevious(daString : String){
		currSibling = currSibling - 1;
		if ( currSibling < 0 ){
			currSibling = daSubParent.transform.childCount - 1;
		}
		if ( daSubParent.transform.GetChild(currSibling).name.Substring(5,5) == daString ){
			daMainObject = daSubParent.transform.GetChild(currSibling).gameObject;
		} else {
			for (var i = currSibling; i >= 0; i--) {
				if ( daSubParent.transform.GetChild(i).name.Substring(5,5) == daString ){
					daMainObject = daSubParent.transform.GetChild(i).gameObject;
					break;
				}
			}
		}
		
		RefreshPosition(daMainObject);
	}

	function SelectNext(daString : String){
		currSibling = currSibling + 1;
		if ( currSibling >= daSubParent.transform.childCount ){
			currSibling = 0;
		}
		if ( daSubParent.transform.GetChild(currSibling).name.Substring(5,5) == daString ){
			daMainObject = daSubParent.transform.GetChild(currSibling).gameObject;
		} else {
			for (var i = currSibling; i < daSubParent.transform.childCount; i++) {
				if ( daSubParent.transform.GetChild(i).name.Substring(5,5) == daString ){
					daMainObject = daSubParent.transform.GetChild(i).gameObject;
					break;
				}
			}
		}		
		RefreshPosition(daMainObject);
	}
	function EffectGreyscale(textureName : String, saveFileName : String){
		FindIfAssetIsAvailable(saveFileName, HPFDpath + "/SavedImages");
		if ( !assetCheckBool ){
			var primaryTexture = AssetDatabase.LoadAssetAtPath(HPFDpath + "/SavedImages/" + textureName + ".png", Sprite) as Sprite;
			if (primaryTexture){
				var texture = new Texture2D((primaryTexture.texture.width), (primaryTexture.texture.height));
				for (var y: int = 0; y < texture.height; y++) {
					for (var x: int = texture.width; x >= 0; x--) {
						texture.SetPixel(x, y, Color(0,0,0,0));
					}
				}
				if (primaryTexture.texture.height > 0){
					for (y = 0; y < primaryTexture.texture.height; y++) {

						for ( x = 0; x < primaryTexture.texture.width; x++) {
							if ( primaryTexture.texture.GetPixel(x, y).a > 0.8 ){
								var textColor 	= primaryTexture.texture.GetPixel(x, y);
								var lowBright : float = 0;
								if (textColor.r <= textColor.g && textColor.r <= textColor.b){
									lowBright = textColor.r;
								} else if (textColor.g <= textColor.r && textColor.g <= textColor.b){
									lowBright = textColor.g;
								} else if (textColor.b <= textColor.r && textColor.b <= textColor.g){
									lowBright = textColor.b;
								}
								texture.SetPixel(x, y, Color(lowBright, lowBright, lowBright, textColor.a));
							}
						}
					}
				}

				texture.Apply(false);

				var bytes 			= texture.EncodeToPNG();
				var path : String 	= HPFDpath + "/SavedImages/" + saveFileName + ".png";
				var file 			= new File.Open(path,FileMode.Create);
				var binary 			= new BinaryWriter(file);
				binary.Write(bytes);
				file.Close();
				AssetDatabase.Refresh();

				var importer : TextureImporter 	= AssetImporter.GetAtPath(HPFDpath + "/SavedImages/" + saveFileName + ".png") as TextureImporter;

				importer.wrapMode 				= TextureWrapMode.Clamp;
				importer.textureType 			= TextureImporterType.Sprite;
				importer.maxTextureSize 		= 2048;
				if(textureName.Substring(22,2) == "MQ"){
					importer.spritePixelsPerUnit = 100/2;
				} else if(textureName.Substring(22,2) == "LQ"){
					importer.spritePixelsPerUnit = 100/3;
				} else {
					importer.spritePixelsPerUnit = 100;
				}
				importer.filterMode 			= FilterMode.Bilinear;
				importer.isReadable 			= true;
				importer.spriteImportMode 		= SpriteImportMode.Single;
				importer.spritePivot 			= Vector2(0,0);

				AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );
			}
		}
		daMainObject.GetComponent.<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath(HPFDpath + "/SavedImages/" + saveFileName + ".png", Sprite) as Sprite;
		EditorSceneManager.MarkAllScenesDirty();
	}

	function AfterSaveMenu(){
		if (daMainObject.GetComponent.<SpriteRenderer>() && daMainObject.GetComponent.<SpriteRenderer>().sprite != null && daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(0,5) == "HPFD_"){
			
			GUILayout.BeginArea (Rect (10,140,300,500));

			texture1 = (daMainObject.GetComponent.<SpriteRenderer>().sprite.texture) as Texture2D;
			tempX = texture1.width;
			tempY = texture1.height;
			if (tempY > 200){
				var tempyDivider = tempY/tempX;
				tempY = 200;
				tempX = tempY/tempyDivider;
			}
			if (tempX > 200){
				tempyDivider = tempX/tempY;
				tempX = 200;
				tempY = tempX/tempyDivider;
			}
			GUILayout.Box(texture1, GUILayout.Width(tempX), GUILayout.Height(tempY));

			if (!boolTwo && daMainObject.transform.name.length > 20){
				boolTwo = true;
				FindIfAssetIsAvailable2(daMainObject.transform.name.Substring(0,21), HPFDpath + "/Prefab");
			}
			if ( assetCheckBool2 ){
				if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_MakeEditable.psd", Texture2D) as Texture2D
							, GUILayout.Width(200), GUILayout.Height(40))) {
					PosX = daMainObject.transform.localPosition.x;
					PosY = daMainObject.transform.localPosition.y;
					PosZ = daMainObject.transform.localPosition.z;
					tempString = daMainObject.transform.name;
					var daCurrIndex : int = daMainObject.transform.GetSiblingIndex();
					DestroyImmediate ( daMainObject.gameObject );
					daMainObject = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Prefab/" + tempString + ".prefab", (typeof(GameObject)))) as GameObject;
					daMainObject.transform.SetParent(daSubParent.transform, false);
					daMainObject.transform.SetSiblingIndex (daCurrIndex);

					RefreshPosition(daMainObject);
				}
			} else {
				EditorGUILayout.LabelField( "Prefab file not found, it may have been deleted" );
				boolTwo = false;
			}
			if (daMainObject.GetComponent.<SpriteRenderer>() && daMainObject.GetComponent.<SpriteRenderer>().sprite.name.length >= 24){
				switch (daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(22,2)){
					case "HQ":
						EditorGUILayout.LabelField( "Current Quality: HIGH Quality" );
					break;
					case "MQ":
						EditorGUILayout.LabelField( "Current Quality: MEDIUM Quality" );
					break;
					case "LQ":
						EditorGUILayout.LabelField( "Current Quality: LOW Quality" );
					break;
				}
				EditorGUILayout.LabelField( "Image Size: " + daMainObject.GetComponent.<SpriteRenderer>().sprite.texture.width + " x " + daMainObject.GetComponent.<SpriteRenderer>().sprite.texture.height );

				if (daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(21,3) == "_MQ" || 
					daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(21,3) == "_LQ" ){
					if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_QualityH.psd", Texture2D) as Texture2D
								, GUILayout.Width(200), GUILayout.Height(40))) {
						FindIfAssetIsAvailable(daMainObject.transform.name.Substring(0,21) + "_HQ", HPFDpath + "/SavedImages");
						if (assetCheckBool){
							daMainObject.GetComponent.<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath(HPFDpath + "/SavedImages/" + daMainObject.transform.name.Substring(0,21) + "_HQ" + ".png", Sprite) as Sprite;
						}
						boolTwo = false;
					}
				}
				if (daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(21,3) == "_HQ" || 
					daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(21,3) == "_LQ" ){
					if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_QualityM.psd", Texture2D) as Texture2D
								, GUILayout.Width(200), GUILayout.Height(40))) {
						FindIfAssetIsAvailable(daMainObject.transform.name.Substring(0,21) + "_MQ", HPFDpath + "/SavedImages");
						if (!assetCheckBool){
							LowerQualityTexture( daMainObject.transform.name.Substring(0,21) + "_HQ", daMainObject.transform.name.Substring(0,21) + "_MQ", 2 );
						}
						daMainObject.GetComponent.<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath(HPFDpath + "/SavedImages/" + daMainObject.transform.name.Substring(0,21) + "_MQ" + ".png", Sprite) as Sprite;
						boolTwo = false;
					}
				}
				if (daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(21,3) == "_HQ" || 
					daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(21,3) == "_MQ" ){
					if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_QualityL.psd", Texture2D) as Texture2D
								, GUILayout.Width(200), GUILayout.Height(40))) {
						FindIfAssetIsAvailable(daMainObject.transform.name.Substring(0,21) + "_LQ", HPFDpath + "/SavedImages");
						if (!assetCheckBool){
							LowerQualityTexture( daMainObject.transform.name.Substring(0,21) + "_HQ", daMainObject.transform.name.Substring(0,21) + "_LQ", 3 );
						}
						daMainObject.GetComponent.<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath(HPFDpath + "/SavedImages/" + daMainObject.transform.name.Substring(0,21) + "_LQ" + ".png", Sprite) as Sprite;
						boolTwo = false;
					}
				}
				if (daMainObject.GetComponent.<SpriteRenderer>().sprite.name.length == 25){
					if (daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(24,1) == "G"){
						if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_ChangeNormal.psd", Texture2D) as Texture2D
									, GUILayout.Width(200), GUILayout.Height(40))) {
							FindIfAssetIsAvailable(daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(0,24), HPFDpath + "/SavedImages");
							if (assetCheckBool){
								daMainObject.GetComponent.<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath(HPFDpath + "/SavedImages/" + daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(0,24) + ".png", Sprite) as Sprite;
							}
							boolTwo = false;
						}
					}
				} else {
					if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_Greyscale.psd", Texture2D) as Texture2D
									, GUILayout.Width(200), GUILayout.Height(40))) {
						EffectGreyscale(daMainObject.GetComponent.<SpriteRenderer>().sprite.name, daMainObject.GetComponent.<SpriteRenderer>().sprite.name.Substring(0,24) + "G");
					}
				}
			} else {
				if(GUILayout.Button(AssetDatabase.LoadAssetAtPath(HPFDpath + "/Editor/WizardImages/Btn_SaveTexture.psd", Texture2D) as Texture2D
										, GUILayout.Width(200), GUILayout.Height(40))) {
					assetCheckBool = false;
					if ( daMainObject.transform.name.Length > 20 ){
						FindIfAssetIsAvailable(daMainObject.transform.name.Substring(0,21), HPFDpath + "/Prefab");
					}
					if (assetCheckBool){
						DeleteAssetsWithName( daMainObject.transform.name.Substring(0,21), HPFDpath + "/SavedImages" );
						SaveTextureToFile(( daMainObject.transform.name.Substring(0,21)), 2048, 2048);
					} else {
						SaveTextureToFile(( daMainObject.transform.name.Substring(0,11) + System.DateTime.UtcNow.ToString("yyddHHmmss")), 2048, 2048);
					}

					RefreshPosition(daMainObject);
				}
			}
			GUILayout.EndArea ();
		} else {
			boolTwo = false;
			EditorGUILayout.LabelField( "####ERROR: Sprite not found####" );
		}
	}
}

