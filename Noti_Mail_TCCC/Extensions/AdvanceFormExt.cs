using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Noti_Mail_TCCC
{
    class AdvanceFormExt
    {
        public class RootObject
        {
            public List<Item> Items { get; set; }
        }

        public class Item
        {
            public string Id { get; set; }
            public List<Layout> Layout { get; set; }
        }

        public class Layout
        {
            public Template Template { get; set; }
            public object Data { get; set; }
            public string Guid { get; set; }
            public bool IsShow { get; set; }
        }

        public class Template
        {
            public string Type { get; set; }
            public string Label { get; set; }
            public string Alter { get; set; }
            public Attribute Attribute { get; set; }
        }

        public class Attribute
        {
            public string Require { get; set; }
            public string Description { get; set; }
            public string Length { get; set; }
            public string Default { get; set; }
            public string Readonly { get; set; }
            public Date Date { get; set; }
            public Time Time { get; set; }
            public List<Column> column { get; set; }
        }

        public class Column
        {
            public string label { get; set; }
            public string alter { get; set; }
            public Control control { get; set; }
        }

        public class Control
        {
            public Template template { get; set; }
            public DataControl Data { get; set; }
        }

        public class Date
        {
            public string Use { get; set; }
            public string UseDate { get; set; }
            public string FullYear { get; set; }
            public string Symbol { get; set; }
        }

        public class Time
        {
            public string Use { get; set; }
            public string UseSecond { get; set; }
            public string Symbol { get; set; }
        }

        public class Data
        {
            public string value { get; set; }
            public List<List<Row>> row { get; set; }
        }
        public class DataControl
        {
            public Value value { get; set; }
        }
        public class Value
        {
            public List<Item> Items { get; set; }
        }
        public class Row
        {
            public string label { get; set; }
            public string value { get; set; }
        }

        public class AdvanceForm
        {
            public string type { get; set; }
            public string alter { get; set; }
            public string guid { get; set; }
            public string label { get; set; }
            public string value { get; set; }
            public List<List<AdvanceFormRow>> row { get; set; }
        }

        public class AdvanceFormRow
        {
            public string label { get; set; }
            public string value { get; set; }
        }

        public static List<AdvanceForm> ToList(string memo)
        {
            List<AdvanceForm> listadvance = new List<AdvanceForm>();

            if (!string.IsNullOrEmpty(memo))
            {
                var data = JsonConvert.DeserializeObject<RootObject>(memo);

                List<Dictionary<string, string>> attributes = new List<Dictionary<string, string>>();


                foreach (var item in data.Items)
                {
                    foreach (var component in item.Layout)
                    {
                        List<List<AdvanceFormRow>> row = new List<List<AdvanceFormRow>>();

                        string guid = component.Guid;
                        string label = component.Template.Label;
                        string type = component.Template.Type;
                        var value = "";

                        try
                        {
                            var dataValue = JsonConvert.DeserializeObject<Data>(component.Data.ToString());

                            var itemData = dataValue;

                            value = itemData.value;

                            if (itemData.row != null && itemData.row.Any())
                            {
                                if (component.Template.Attribute.column.Any())
                                {
                                    var column = component.Template.Attribute.column;
                                    foreach (var itemrow in itemData.row)
                                    {
                                        int index = 0;
                                        var listRow = new List<AdvanceFormRow>();
                                        foreach (var itemx in itemrow)
                                        {
                                            itemx.label = column[index].label;
                                            listRow.Add(new AdvanceFormRow
                                            {
                                                label = itemx.label,
                                                value = itemx.value,
                                            });
                                            index++;
                                        }

                                        row.Add(listRow);
                                    }
                                }
                            }

                            listadvance.Add(new AdvanceForm
                            {
                                label = label,
                                value = value,
                                row = row,
                                guid = guid,
                                type = type
                            });
                        }
                        catch
                        {
                            try
                            {
                                var dataValue = JsonConvert.DeserializeObject<List<Data>>(component.Data.ToString());

                                var listItemData = dataValue;

                                foreach (var itemData in listItemData)
                                {
                                    value = itemData.value;

                                    if (itemData.row != null && itemData.row.Any())
                                    {
                                        if (component.Template.Attribute.column.Any())
                                        {
                                            var column = component.Template.Attribute.column;
                                            foreach (var itemrow in itemData.row)
                                            {
                                                int index = 0;
                                                var listRow = new List<AdvanceFormRow>();
                                                foreach (var itemx in itemrow)
                                                {
                                                    itemx.label = column[index].label;
                                                    listRow.Add(new AdvanceFormRow
                                                    {
                                                        label = itemx.label,
                                                        value = itemx.value,
                                                    });
                                                    index++;
                                                }

                                                row.Add(listRow);
                                            }
                                        }
                                    }

                                    listadvance.Add(new AdvanceForm
                                    {
                                        label = label,
                                        value = value,
                                        row = row,
                                        guid = guid,
                                        type = type
                                    });
                                }
                            }

                            catch
                            {

                            }
                        }
                    }
                }
            }

            return listadvance;
        }

        public static string ReplaceDataProcess(string DestAdvanceForm, string Value, string label)
        {
            var jsonAdvanceForm = JsonConvert.DeserializeObject<JObject>(DestAdvanceForm);
            JArray itemsArray = (JArray)jsonAdvanceForm["items"];
            foreach (JObject jItems in itemsArray)
            {
                JArray jLayoutArray = (JArray)jItems["layout"];

                if (jLayoutArray.Count >= 1)
                {
                    JObject jTemplateL = (JObject)jLayoutArray[0]["template"];

                    if ((String)jTemplateL["label"] == label)
                    {

                        JObject jData = (JObject)jLayoutArray[0]["data"];
                        if (jData != null)
                        {
                            jData["value"] = Value;
                        }
                    }

                    if (jLayoutArray.Count > 1)
                    {
                        JObject jTemplateR = (JObject)jLayoutArray[1]["template"];

                        if ((String)jTemplateR["label"] == label)
                        {

                            JObject jData = (JObject)jLayoutArray[1]["data"];
                            if (jData != null)
                            {
                                jData["value"] = Value;
                            }
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(jsonAdvanceForm);
        }
    }
}
