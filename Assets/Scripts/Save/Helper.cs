using System.IO;
using System.Xml.Serialization;

public static class Helper
{
  
    // -- Serialize
    public static string Serialize<T>(this T toSerialize)
    {
        // -- Instancia um XmlSerializer
        XmlSerializer xml = new XmlSerializer(typeof(T));
        // -- Instancia um StringWriter
        StringWriter writer = new StringWriter();
        // -- Serializa em xml o writer com o conteúdo passado por parâmetro (class GameConfig)
        xml.Serialize(writer, toSerialize);
        // -- Retorna o writer serializado
        return writer.ToString();
    }

    // -- De-serialize
    public static T Deserialize<T>(this string toDeserialize)
    {
        // -- Instancia um XmlSerializer
        XmlSerializer xml = new XmlSerializer(typeof(T));
        // -- Instancia um StringReader
        StringReader reader = new StringReader(toDeserialize);
        // -- Retorna o xml deserializado do tipo T, passado por parâmetro (class GameConfig)
        return (T)xml.Deserialize(reader);
    }

}
