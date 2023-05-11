using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text.Json;
using Avalonia.Controls.Shapes;
using DynamicData.Diagnostics;

namespace Timetable.Models
{
    class JSONSerializer<T>
    {
        public static void Save(string path, T item)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                JsonSerializer.Serialize<T>(fs, item,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
            }

        }
        public static T? Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                return JsonSerializer.Deserialize<T>(fs, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }

        }
        public static object? Json2obj(string jsonString)
        {
            jsonString = jsonString.Trim();
            if (jsonString.Length == 0) return null;

            object? data;
            if (jsonString[0] == '[') data = JsonSerializer.Deserialize<object?[]>(jsonString);
            else if (jsonString[0] == '{') data = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonString);
            else return null;

            return JsonHandler(data);
        }
        private static object? JsonHandler(object? obj)
        {
            if (obj == null) return null;

            if (obj is object?[] @list) return @list.Select(JsonHandler).ToArray();
            if (obj is Dictionary<string, object?> @dict)
            {
                return new Dictionary<string, object?>(@dict.Select(pair => new KeyValuePair<string, object?>(pair.Key, JsonHandler(pair.Value))));
            }
            if (obj is JsonElement @item)
            {
                switch (@item.ValueKind)
                {
                    case JsonValueKind.Undefined: return null;
                    case JsonValueKind.Object:
                        Dictionary<string, object?> res = new();
                        foreach (var el in @item.EnumerateObject()) res[el.Name] = JsonHandler(el.Value);
                        return res;
                    case JsonValueKind.Array:
                        object?[] res2 = @item.EnumerateArray().Select(item => JsonHandler((object?)item)).ToArray();
                        return res2;
                    case JsonValueKind.String:
                        var s = JsonHandler(@item.GetString() ?? "");
                        // Log.Write("JS: '" + @item.GetString() + "' -> '" + s + "'");
                        return s;
                    case JsonValueKind.Number:
                        if (@item.ToString().Contains('.')) return @item.GetDouble();
                        // Иначе это целое число
                        long a = @item.GetInt64();
                        int b = @item.GetInt32();
                        // short c = @item.GetInt16();
                        if (a != b) return a;
                        // if (b != c) return b;
                        return b;
                    case JsonValueKind.True: return true;
                    case JsonValueKind.False: return false;
                    case JsonValueKind.Null: return null;
                }
            }
            return obj;
        }
    }
}