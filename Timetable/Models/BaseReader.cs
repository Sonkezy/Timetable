using Avalonia.Media.Imaging;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Timetable.Models
{
    public class BaseReader
    {
        public TableItem[][] data = Array.Empty<TableItem[]>();
        public Dictionary<string, Bitmap> images = new();
        public const int days_num = 6;

        public BaseReader()
        {
            string path = (Directory.GetCurrentDirectory().Contains("Timetable") ? "../../.." : "Timetable") + "/Assets/storage.db";
            using var con = new SqliteConnection("Data Source=" + path);
            con.Open();

            using (var reader2 = new SqliteCommand("SELECT * FROM images", con).ExecuteReader())
            {
                if (!reader2.HasRows) return;

                while (reader2.Read())
                {
                    var row = Enumerable.Range(0, reader2.VisibleFieldCount).Select(x => reader2[x]).ToArray();
                    var id = (string)row[0];
                    images[id] = Base64toBitmap(((string)row[1]));
                }
            }

            using var reader = new SqliteCommand("SELECT * FROM content", con).ExecuteReader();
            if (!reader.HasRows) return;

            var res = new List<TableItem>[days_num];
            for (int i = 0; i < days_num; i++) res[i] = new();

            while (reader.Read())
            {
                var row = Enumerable.Range(0, reader.VisibleFieldCount).Select(x => reader[x]).ToArray();
                long day = (long)row[1];
                var obj = JSONSerializer<object>.Json2obj((string)row[2]) ?? throw new System.Exception("Чё?!");

                res[day].Add(new TableItem((object[])obj, this));
            }
            data = res.Select(x => x.ToArray()).ToArray();
        }
        public static Bitmap Base64toBitmap(string data)
        {
            byte[] bytes = Convert.FromBase64String(data.Split(";base64,", 2)[1]);
            Stream stream = new MemoryStream(bytes);
            return new Bitmap(stream);
        }

        // Рандомит некоторые вещи, связанные с текущим временем ;'-}
        long days_offset = 0;
        public void TimeBomb()
        {
            var dt = DateTime.Now;
            var delta = dt - new DateTime(1970, 1, 1, 0, 0, 0);
            days_offset = (long)delta.TotalDays - 19474;
            // Log.Write("days: " + days_offset + " mins: " + delta.TotalMinutes % (24 * 60));
            int num_day = 0;
            int mins_offset = (int)(days_offset * 24 * 60 + delta.TotalMinutes) % (days_num * 24 * 60);
            foreach (var day in data)
            {
                foreach (var line in day) line.RecalcTime(num_day, mins_offset);
                num_day++;
            }
        }

        public TableItem[] GetItems(bool selected, int selected2)
        {
            TimeBomb();
            return data[(selected2 + days_offset) % days_num].Where(x => x.IsDeparture != selected).ToArray();
        }
    }
}
