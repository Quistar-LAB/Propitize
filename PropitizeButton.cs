using System.IO;
using System.Reflection;
using ColossalFramework.UI;
using UnityEngine;
using MoveIt;
using UIUtils = SamsamTS.UIUtils;

namespace Propitize
{
    public class PropitizeButton : UIPanel
    {
        static private UIButton m_propitize_button;
        // Copied over from Moveit mod
        static public UIButton CreateSubButton(UIToolOptionPanel parent, string name, string tooltip, string fgSprite)
        {
            m_propitize_button = parent.m_viewOptions.AddUIComponent<UIButton>();
            m_propitize_button.name = name;
            m_propitize_button.atlas = GetIconsAtlas();
            m_propitize_button.tooltip = tooltip;
            m_propitize_button.playAudioEvents = true;

            m_propitize_button.size = new Vector2(36, 36);

            m_propitize_button.normalBgSprite = "OptionBase";
            m_propitize_button.hoveredBgSprite = "OptionBaseHovered";
            m_propitize_button.pressedBgSprite = "OptionBasePressed";
            m_propitize_button.disabledBgSprite = "OptionBaseDisabled";

            m_propitize_button.normalFgSprite = fgSprite;

            m_propitize_button.relativePosition = new Vector3(4f, 112f);

            m_propitize_button.eventClicked += (c, p) =>
            {
                PropitizeAction.instance.StartConvertAction();
            };

            parent.m_viewOptions.height += 36;
            parent.m_viewOptions.absolutePosition += new Vector3(0, -36);

            return m_propitize_button;
        }

        static internal UITextureAtlas GetIconsAtlas()
        {
            UITextureAtlas atlas = UIUtils.GetAtlas("Ingame");
            Texture2D[] textures =
            {
                atlas["OptionBase"].texture,
                atlas["OptionBaseHovered"].texture,
                atlas["OptionBasePressed"].texture,
                atlas["OptionBaseDisabled"].texture,
                atlas["OptionBaseFocused"].texture,
                atlas["OptionsDropboxListbox"].texture,
                atlas["OptionsDropboxListboxHovered"].texture,
                atlas["OptionsDropboxListboxFocused"].texture
            };

            UITextureAtlas loadedAtlas = CreateTextureAtlas("Propitize", "Propitize.icon.");
            AddTexturesInAtlas(loadedAtlas, textures);

            return loadedAtlas;
        }

        static private UITextureAtlas CreateTextureAtlas(string spriteName, string assemblyPath)
        {
            const int maxSize = 1024;
            Texture2D texture2D = new Texture2D(maxSize, maxSize, TextureFormat.ARGB32, false);
            Texture2D[] textures = new Texture2D[1];
            Rect[] regions = new Rect[1];

            textures[0] = loadTextureFromAssembly(assemblyPath + spriteName + ".png");

            regions = texture2D.PackTextures(textures, 4, maxSize);

            UITextureAtlas textureAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            Material material = Object.Instantiate(UIView.GetAView().defaultAtlas.material);
            material.mainTexture = texture2D;
            textureAtlas.material = material;
            textureAtlas.name = spriteName;

            UITextureAtlas.SpriteInfo item = new UITextureAtlas.SpriteInfo
            {
                name = spriteName,
                texture = textures[0],
                region = regions[0],
            };
            textureAtlas.AddSprite(item);
            
            return textureAtlas;
        }

        private static void AddTexturesInAtlas(UITextureAtlas atlas, Texture2D[] newTextures, bool locked = false)
        {
            Texture2D[] textures = new Texture2D[atlas.count + newTextures.Length];

            for (int i = 0; i < atlas.count; i++)
            {
                Texture2D texture2D = atlas.sprites[i].texture;

                if (locked)
                {
                    // Locked textures workaround
                    RenderTexture renderTexture = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 0);
                    Graphics.Blit(texture2D, renderTexture);
                    RenderTexture active = RenderTexture.active;
                    texture2D = new Texture2D(renderTexture.width, renderTexture.height);
                    RenderTexture.active = renderTexture;
                    texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
                    texture2D.Apply();
                    RenderTexture.active = active;
                    RenderTexture.ReleaseTemporary(renderTexture);
                }

                textures[i] = texture2D;
                textures[i].name = atlas.sprites[i].name;
            }

            for (int i = 0; i < newTextures.Length; i++)
                textures[atlas.count + i] = newTextures[i];

            Rect[] regions = atlas.texture.PackTextures(textures, atlas.padding, 4096, false);

            atlas.sprites.Clear();

            for (int i = 0; i < textures.Length; i++)
            {
                UITextureAtlas.SpriteInfo spriteInfo = atlas[textures[i].name];
                atlas.sprites.Add(new UITextureAtlas.SpriteInfo
                {
                    texture = textures[i],
                    name = textures[i].name,
                    border = (spriteInfo != null) ? spriteInfo.border : new RectOffset(),
                    region = regions[i]
                });
            }

            atlas.RebuildIndexes();
        }

        private static Texture2D loadTextureFromAssembly(string path)
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);

            byte[] array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);

            Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            texture2D.LoadImage(array);

            return texture2D;
        }
    }
}