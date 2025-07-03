// Prototype Collection - Street Props
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AmplifyCreations.PrototypeCollection.StreetProps
{
	[InitializeOnLoad]
	public class StartScreen : EditorWindow
	{
		private static readonly string BIRPAssetsGUID = "5f78b443fb7cb9f478c4b3da85b8d8e5";
		private static readonly string URPAssetsGUID = "aa3bab5027749a648aa738f1cd165bc2";
		private static readonly string HDRPAssetsGUID = "da115db3158e72c47ae5ad3850c0baa4";

		private static readonly string DiscordURL = "https://discord.gg/SbNs7zK";
		private static readonly string StoreURL = "https://assetstore.unity.com/publishers/707?aid=1011lPwI&pubref=AF3D";

		private static readonly string WindowTitle = "Prototype Collection - Street Props";

		private static readonly GUIContent SamplesTitle = new GUIContent( "Street Props", "Import samples according to you project rendering pipeline" );

		private const string DisplayDialogTitle = "Import Street Props";
		private const string DisplayDialogText = "This will import the samples for the selected pipeline, please make sure the pipeline is properly installed and set before importing the samples.\n\nContinue?";

		private static readonly GUIContent Body = new GUIContent( "Enjoy our free collection of Street Props. Join the discussion, tell us\nhow you used them and learn more about our other Assets." );

		private bool m_startup = true;

		[NonSerialized] private Texture m_packageIcon = null;
		[NonSerialized] private Texture m_webIcon = null;

		private GUIContent m_BIRPButton = null;
		private GUIContent m_HDRPButton = null;
		private GUIContent m_URPButton = null;

		private GUIContent m_DiscordButton = null;
		private GUIContent m_StoreButton = null;

		[NonSerialized] private GUIStyle m_buttonStyle = null;
		[NonSerialized] private GUIStyle m_labelStyle = null;
		[NonSerialized] private GUIStyle m_linkStyle = null;

		private struct Banner
		{
			public string desc;
			public string url;
			public string imageGUID;
			public Texture2D image;

			public Banner( string desc, string url, string imageGUID )
			{
				this.desc = desc;
				this.url = url;
				this.imageGUID = imageGUID;
				this.image = null;
			}
		}

		private static readonly Banner[] Banners = new Banner[]
		{
			new Banner(
				"Save Big with Amplify Bundle!",
				"https://assetstore.unity.com/packages/tools/visual-scripting/amplify-bundle-173849?aid=1011lPwI&pubref=AF3D",
				"2e6f99c42f698fb40b05c220d90becbc"
			),
			new Banner(
				"Amplify Shader editor, award-winnning editor for any project.",
				"https://assetstore.unity.com/packages/tools/visual-scripting/amplify-shader-editor-68570?aid=1011lPwI&pubref=AF3D",
				"7d025e5606137cc4ebae0c93f43f677e"
			),
			new Banner(
				"Amplify Impostors, 1-Click optimization.",
				"https://assetstore.unity.com/packages/tools/utilities/amplify-impostors-119877?aid=1011lPwI&pubref=AF3D",
				"46f890fe7bef64f46a7ad9146a043887"
			),
			new Banner(
				"Amplify Animation Pack, 600+ Anims, Blender Source and Character Controller.",
				"https://assetstore.unity.com/packages/3d/animations/amplify-animation-pack-207911?aid=1011lPwI&pubref=AF3D",
				"7cd417e77a5dd954f81f4cbb93786186"
			)
		};

		static StartScreen()
		{
			if ( !EditorPrefs.HasKey( Preferences.PrefStartUp ) )
			{
				EditorPrefs.SetBool( Preferences.PrefStartUp, true );
			}

			if ( EditorPrefs.GetBool( Preferences.PrefStartUp ) )
			{
				EditorApplication.delayCall += Init;
			}
		}

		[MenuItem( "Window/Prototype Collection/Street Props", false, 1998 )]
		public static void Init()
		{
			StartScreen window = ( StartScreen )GetWindow( typeof( StartScreen ), true, WindowTitle );
			window.minSize = new Vector2( 670, 700 );
			window.maxSize = new Vector2( 670, 700 );
			window.Show();
		}

		private void OnEnable()
		{
			m_startup = EditorPrefs.GetBool( Preferences.PrefStartUp, true );

			if ( m_packageIcon == null )
			{
				m_packageIcon = EditorGUIUtility.IconContent( "BuildSettings.Editor.Small" ).image;
				m_BIRPButton = new GUIContent( " Built-In Assets", m_packageIcon );
				m_HDRPButton = new GUIContent( " HDRP Assets", m_packageIcon );
				m_URPButton = new GUIContent( " URP Assets", m_packageIcon );
			}

			if ( m_webIcon == null )
			{
				m_webIcon = EditorGUIUtility.IconContent( "BuildSettings.Web.Small" ).image;
				m_DiscordButton = new GUIContent( " Discord", m_webIcon );
				m_StoreButton = new GUIContent( " Asset Store", m_webIcon );
			}
		}

		public void OnGUI()
		{
			if ( m_buttonStyle == null )
			{
				m_buttonStyle = new GUIStyle( GUI.skin.button );
				m_buttonStyle.alignment = TextAnchor.MiddleLeft;
			}

			if ( m_labelStyle == null )
			{
				m_labelStyle = new GUIStyle( "BoldLabel" );
				m_labelStyle.margin = new RectOffset( 4, 4, 4, 4 );
				m_labelStyle.padding = new RectOffset( 2, 2, 2, 2 );
				m_labelStyle.fontSize = 13;
			}

			if ( m_linkStyle == null )
			{
				var inv = AssetDatabase.LoadAssetAtPath<Texture2D>( AssetDatabase.GUIDToAssetPath( "1004d06b4b28f5943abdf2313a22790a" ) ); // find a better solution for transparent buttons
				m_linkStyle = new GUIStyle();
				m_linkStyle.normal.textColor = new Color( 0.2980392f, 0.4901961f, 1f );
				m_linkStyle.hover.textColor = Color.white;
				m_linkStyle.active.textColor = Color.grey;
				m_linkStyle.margin.top = 3;
				m_linkStyle.margin.bottom = 2;
				m_linkStyle.hover.background = inv;
				m_linkStyle.active.background = inv;
			}

			EditorGUILayout.BeginHorizontal( GUIStyle.none, GUILayout.ExpandWidth( true ) );
			{
				/////////////////////////////////////////////////////////////////////////////////////
				// LEFT COLUMN
				/////////////////////////////////////////////////////////////////////////////////////
				EditorGUILayout.BeginVertical( GUILayout.Width( 175 ) );
				{
					GUILayout.Label( SamplesTitle, m_labelStyle );

					if ( GUILayout.Button( m_HDRPButton, m_buttonStyle ) )
					{
						if ( EditorUtility.DisplayDialog( DisplayDialogTitle, DisplayDialogText, "Yes", "No" ) )
						{
							AssetDatabase.ImportPackage( AssetDatabase.GUIDToAssetPath( HDRPAssetsGUID ), false );
						}
					}

					if ( GUILayout.Button( m_URPButton, m_buttonStyle ) )
					{
						if ( EditorUtility.DisplayDialog( DisplayDialogTitle, DisplayDialogText, "Yes", "No" ) )
						{
							AssetDatabase.ImportPackage( AssetDatabase.GUIDToAssetPath( URPAssetsGUID ), false );
						}
					}

					if ( GUILayout.Button( m_BIRPButton, m_buttonStyle ) )
					{
						if ( EditorUtility.DisplayDialog( DisplayDialogTitle, DisplayDialogText, "Yes", "No" ) )
						{
							AssetDatabase.ImportPackage( AssetDatabase.GUIDToAssetPath( BIRPAssetsGUID ), false );
						}
					}
				}
				EditorGUILayout.EndVertical();

				/////////////////////////////////////////////////////////////////////////////////////
				// RIGHT COLUMN
				/////////////////////////////////////////////////////////////////////////////////////
				EditorGUILayout.BeginVertical( GUILayout.Width( 470 ), GUILayout.ExpandHeight( true ) );
				{
					GUILayout.Space( 20 );
					GUILayout.BeginHorizontal();
					{
						GUILayout.BeginVertical();
						{
							GUILayout.Label( Body );
							GUILayout.Space( 2 );
						}
						GUILayout.EndVertical();
					}
					GUILayout.EndHorizontal();

					GUILayout.Space( 13 );

					//Discord/Forum
					GUILayout.BeginHorizontal( GUILayout.ExpandWidth( true ) );
					{
						if ( GUILayout.Button( m_DiscordButton, GUILayout.ExpandWidth( true ) ) )
						{
							Application.OpenURL( DiscordURL );
						}
						if ( GUILayout.Button( m_StoreButton, GUILayout.ExpandWidth( true ) ) )
						{
							Application.OpenURL( StoreURL );
						}
					}

					Color bufferColor = GUI.color;
					Color backgroundColor = GUI.backgroundColor;

					GUI.backgroundColor = new Color( 0, 0, 0, 0 );
					GUILayout.EndHorizontal();

					GUI.color = bufferColor;

					GUILayout.Space( 20 );

					for ( int i = 0; i < Banners.Length; i++ )
					{
						GUILayout.Label( Banners[ i ].desc );

						if ( Banners[ i ].image == null )
						{
							Banners[ i ].image = AssetDatabase.LoadAssetAtPath<Texture2D>( AssetDatabase.GUIDToAssetPath( Banners[ i ].imageGUID ) );
						}

						if ( GUILayout.Button( new GUIContent( Banners[ i ].image ), m_linkStyle, GUILayout.Width( 470 ) ) )
						{
							Application.OpenURL( Banners[ i ].url );
						}

						GUILayout.Space( 15 );
					}

					GUI.backgroundColor = backgroundColor;
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal( "ProjectBrowserBottomBarBg", GUILayout.ExpandWidth( true ), GUILayout.Height( 22 ) );
			{
				GUILayout.FlexibleSpace();
				EditorGUI.BeginChangeCheck();
				var cache = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 100;
				m_startup = EditorGUILayout.Toggle( "Show At Startup", m_startup, GUILayout.Width( 120 ) );
				EditorGUIUtility.labelWidth = cache;
				if ( EditorGUI.EndChangeCheck() )
				{
					EditorPrefs.SetBool( Preferences.PrefStartUp, m_startup );
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}