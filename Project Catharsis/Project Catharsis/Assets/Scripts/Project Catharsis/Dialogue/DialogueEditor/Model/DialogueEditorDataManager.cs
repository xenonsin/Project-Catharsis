using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Catharsis.DialogueEditor.Model
{
    public class DialogueEditorDataManager
    {
        public DialogueEditorData data;

        public DialogueEditorDataManager()
        {
            data = new DialogueEditorData();
        }

        public void Save(string file)
        {
            if (file.StartsWith(Application.dataPath))
                AssetDatabase.DeleteAsset(file);
            XmlSerializer serializer = new XmlSerializer(typeof(DialogueEditorData));
            TextWriter textWriter = new StreamWriter(file);
            serializer.Serialize(textWriter, data);
            textWriter.Close();
            AssetDatabase.Refresh();
        }

        public void Load(string file)
        {

            data = null;
            XmlSerializer deserializer = new XmlSerializer(typeof(DialogueEditorData));
            TextReader textReader = new StreamReader(file);
            data = (DialogueEditorData)deserializer.Deserialize(textReader);
            textReader.Close();
            
        }
    }
}