using Newtonsoft.Json;
using System.Collections;
using System.Resources;
using System.Transactions;

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
            var linkRestEn = "E:/TanTam/wmsproject/trunk/WebApp/WMS2014/TGDD.WMS.Utils/Resource.en-US.resx";
            var linkRestVN = "E:/TanTam/wmsproject/trunk/WebApp/WMS2014/TGDD.WMS.Utils/Resource.resx";
            List<string> resultTex = textBox1.Text.Split(',').ToList();
            var tienTo = textBox2.Text.ToString();
            var xsss= textBox1.Text.ToString();
            var texts = new List<string>();//texts
            texts.AddRange(resultTex);
            var resultDic = new Dictionary<string, string>();
            var outPuttextDic = new outputText();
            var urltexts = texts.Select(_ =>
            {
                return string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=en&dt=t&q={0}", _);
            }).ToList();

            string url = String.Format
            ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
             "vi", "en", Uri.EscapeUriString(xsss));
            HttpClient httpClient = new HttpClient();
            string result = httpClient.GetStringAsync(url).Result;
            var jsonData = JsonConvert.DeserializeObject<List<dynamic>>(result);
            var translationItems = jsonData[0];
            string translation = "";
            var dtranslation = "";
            foreach (object item in translationItems)
            {
                IEnumerable translationLineObject = item as IEnumerable;
                IEnumerator translationLineString = translationLineObject.GetEnumerator();
                translationLineString.MoveNext();
                translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
                translationLineString.MoveNext();
                dtranslation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
            }
            if (translation.Length > 1)
            {
                translation = translation.Substring(1);
            }

            List<string> resultTrsEng = translation.Split(',').ToList();
            List<string> resultTrsVN = dtranslation.Split(',').ToList();
            for(var i = 0; i < resultTrsEng.Count(); i++)
            {
                outPuttextDic.TextEN.Add(tienTo + "_" + resultTrsEng[i], resultTrsEng[i]);
                outPuttextDic.TextVI.Add(tienTo + "_" + resultTrsEng[i], resultTrsVN[i]);
            }
            using var readllE = new ResXResourceReader(linkRestEn);
            using var readllV = new ResXResourceReader(linkRestVN);
            var x = readllE.GetEnumerator();//GetEnumerator tiếng anh đã có sẵn
            var xv = readllV.GetEnumerator();//GetEnumerator tiếng việt đã có sẵn
            var xc = new outputText();
            while (x.MoveNext())
            {
                try
                {
                    outPuttextDic.TextEN.Add((string)x.Key, (string)x.Value);
                }
                catch (Exception ex)
                {
                    x.MoveNext();
                }
            }
           
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
            using (ResXResourceWriter resx = new ResXResourceWriter(linkRestEn))
            {
                outPuttextDic.TextEN.Select(x =>
                {
                    resx.AddResource(x.Key, x.Value);
                    return xc;
                }).ToList();

            }
            using (ResXResourceWriter resxV = new ResXResourceWriter(linkRestVN))
            {
                outPuttextDic.TextVI.Select(x =>
                {
                    resxV.AddResource(x.Key, x.Value);
                    return xc;
                }).ToList();

            }
        }

    

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}