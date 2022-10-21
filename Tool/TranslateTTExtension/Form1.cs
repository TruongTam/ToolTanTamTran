using Newtonsoft.Json;
using System.Collections;
using System.Resources;

namespace TranslateTTExtension
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            List<string> resultTex = textBox1.Text.Split(',').ToList();
            var texts = new List<string>();//texts
            texts.AddRange(resultTex);
            var resultDic = new Dictionary<string, string>();
            var outPuttextDic = new outputText();
            var urltexts = texts.Select(_ =>
            {
                return string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=en&dt=t&q={0}", _);
            }).ToList();
            for (var i = 0; i < urltexts.Count(); i++)
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, urltexts[i]);

                //httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    var result = await httpClient.SendAsync(httpRequest);
                    string results = result.Content.ReadAsStringAsync().Result;
                    var jsonData = JsonConvert.DeserializeObject<List<dynamic>>(results);
                    var translationItems = jsonData[0];
                    string translation = "";
                    foreach (object item in translationItems)
                    {
                        IEnumerable translationLineObject = item as IEnumerable;
                        IEnumerator translationLineString = translationLineObject.GetEnumerator();
                        translationLineString.MoveNext();
                        translation += string.Format("{0}", Convert.ToString(translationLineString.Current));
                    }
                    var textToE = string.Format("{0}_{1}: {2}", resultTex[i], translation.Replace(" ", ""), translation);
                    var textToV = string.Format("{0}_{1}: {2}", resultTex[i], translation.Replace(" ", ""), texts[i]);
                    try
                    {
                        outPuttextDic.TextEN.Add(string.Format("{0}_{1}", textBox2.Text, translation.Replace(" ", "")), translation);

                        outPuttextDic.TextVI.Add(string.Format("{0}_{1}", textBox2.Text, translation.Replace(" ", "")), resultTex[i]);


                        resultDic.Add(textToV, textToE);
                    }
                    catch (Exception ex)
                    {
                        i++;
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        Console.Write(result.StatusCode);
                    }
                    else
                    {
                        Console.Write(result.StatusCode);
                    }
                }
                using var readllE = new ResXResourceReader("E:/TanTam/wmsproject/trunk/WebApp/WMS2014/TGDD.WMS.Utils/Resource.en-US.resx");
                using var readllV = new ResXResourceReader("E:/TanTam/wmsproject/trunk/WebApp/WMS2014/TGDD.WMS.Utils/Resource.resx");
                var x = readllE.GetEnumerator();
                var xc = new outputText();
                while (x.MoveNext())
                {
                    try
                    {
                        outPuttextDic.TextEN.Add((string)x.Key, (string)x.Value);
                        
                        resultDic.Add((string)x.Key, (string)x.Value);
                    }
                    catch (Exception ex)
                    {
                        x.MoveNext();
                    }
                }
                var xv = readllV.GetEnumerator();
                var xcv = new outputText();
                while (xv.MoveNext())
                {
                    try
                    {
                        outPuttextDic.TextVI.Add((string)xv.Key, (string)xv.Value);
                    }
                    catch (Exception ex)
                    {
                        xv.MoveNext();
                    }
                }
                //Chính Sách, Đổi Trả, Thu, Mua
                using (ResXResourceWriter resx = new ResXResourceWriter("E:/TanTam/wmsproject/trunk/WebApp/WMS2014/TGDD.WMS.Utils/Resource.en-US.resx"))
                {
                    outPuttextDic.TextEN.Select(x =>
                    {
                        resx.AddResource(x.Key, x.Value);
                        return xc;
                    }).ToList();

                }
                using (ResXResourceWriter resxV = new ResXResourceWriter("E:/TanTam/wmsproject/trunk/WebApp/WMS2014/TGDD.WMS.Utils/Resource.resx"))
                {
                    outPuttextDic.TextVI.Select(x =>
                    {
                        resxV.AddResource(x.Key, x.Value);
                        return xc;
                    }).ToList();

                }
            };



        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}