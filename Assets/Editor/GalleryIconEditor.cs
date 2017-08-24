using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(GalleryIcon))]
public class GalleryIconEditor:ExtendEditor
{
    public class GalleryProperty
    {
        public GalleryAppInfo info;
        public bool fold;
    }
    List<GalleryProperty> propertys = new List<GalleryProperty>();
    GalleryIcon app;
    int size;
    private void OnEnable()
    {
        
        app = (GalleryIcon)target;
        size = app.infos.Length;
    }
    public override void OnInspectorGUI()
    {
        CreateDelayField(ref size, "케이스 수");
        ArraySizeModify(ref app.infos, size);
        ArraySizeModify(propertys, size);
        if(size==1)
        {
            if (propertys[0].info == null) propertys[0].info = app.infos[0];
            CreateProperty(propertys[0]);
        }
        else
        {
            for(int i=0;i<size;i++)
            {
                if (propertys[i].info == null) propertys[i].info = app.infos[i];
                FoldOut(ref propertys[i].fold, "케이스", i + 1, CreateProperty, propertys[i]);
            }
        }

    }
    void CreateProperty(GalleryProperty property)
    {
        Rect pos = EditorGUILayout.GetControlRect();
        pos.width = 150;
        pos.height = 150;
        GUILayout.Space(150); //공간 확보
        property.info.image = (Texture2D)EditorGUI.ObjectField(pos, property.info.image,typeof(Texture),true);
        Rect textpos = new Rect(pos.x+170, pos.y, 150, 20);
        float value = property.info.PuzzleNumber;
        if (value < 2) value = 2;
        GUI.Label(textpos, "현재 퍼즐수:" + value + "×" + value);
        textpos.y += 20;
        value = GUI.HorizontalSlider(textpos, value, 2, 10);
        float startloc = 150 / value;
        float startpos_x = pos.x + 10;
        Rect widthpos = new Rect(startpos_x, pos.y, 150, 2);
        Rect heightpos = new Rect(startpos_x, pos.y, 2, 150);
        Texture2D line = new Texture2D(100, 100);
        textpos.y += 20;
        textpos.width = 200;
        EditorGUI.LabelField(textpos, pos.ToString());
        Rect notice1 = new Rect(widthpos.x, widthpos.y - 5, 30, 30);
        Rect notice2 = new Rect(heightpos.x - 5, heightpos.y, 30, 30);
        GUIStyle st = new GUIStyle();
        st.normal.textColor = Color.white;
        for(int i=1;i<=value;i++)
        {
            textpos.y += 20;
            EditorGUI.DrawPreviewTexture(widthpos, line);
            EditorGUI.DrawPreviewTexture(heightpos, line);
            EditorGUI.LabelField(textpos, i + ":" + heightpos.x);
            EditorGUI.LabelField(notice1, i+"",st);
            EditorGUI.LabelField(notice2, i + "",st);
            widthpos.y = pos.y + (i / value) * 150;
            heightpos.x = startpos_x+ (i / value) * 150;
            notice1.y = widthpos.y - 5;
            notice2.x = heightpos.x - 5;
        }
        property.info.PuzzleNumber = (int)value;
    }
}
