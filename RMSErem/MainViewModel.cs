using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RMSErem
{
    public class MainViewModel : PropertyChangedBase
    {
        public enum Type
        {
            Reguła,
            Model
        }
        public Dictionary<Type, (string rem, string name)> TypeStringValuesDictionary = new Dictionary<Type, (string, string)>()
               {
                    { Type.Reguła, ("rem_reguła", "reguł" ) },
                    { Type.Model, ("rem_model", "modeli" ) }
               };

        BindableCollection<string> typeList;
        List<string> AllInput;
        int defaultLength;
        int minMargin;


        public BindableCollection<string> TypeList => typeList;
        public string InputText { get; set; }
        public string Result { get; set; }
        public Type SelectedType { get; set; }

        public MainViewModel()
        {
            typeList = new BindableCollection<string>();
            AllInput = new List<string>();
            defaultLength = 50;
            minMargin = 10;

            InsertEnumValuesIntoTypeList();
        }
        public void Generate(ActionExecutionContext sender)
        {

            if (InputText != null && ((sender.EventArgs as KeyEventArgs)?.Key == Key.Enter || sender.Source.GetType() == typeof(Button)))
            {
                AllInput.Add(InputText);
                if (AllInput.FirstOrDefault(x => !(x == "")) != null)
                {
                    Result = GetRMSEComments(AllInput);
                }
                InputText = "";

                NotifyOfPropertyChange(() => Result);
                NotifyOfPropertyChange(() => InputText);
            }
        }
        public void Clear()
        {
            AllInput.Clear();
            Result = "";
            NotifyOfPropertyChange(() => Result);
        }
        private string GetRMSEComments(IEnumerable<string> collection)
        {
            string rem = TypeStringValuesDictionary.FirstOrDefault(x => x.Key == SelectedType).Value.rem;
            string result = "";
            string startString = GetStartString(SelectedType);
            string endString = GetEndString(SelectedType);
            int max = GetLongestPossibleTitle(collection, Math.Max(startString.Length, endString.Length));
            int length = max + minMargin * 2;

            foreach (var item in collection)
            {
                {
                    string title = $"{startString}{item}";
                    (int right, int left) margin = GetMargin(title, max + minMargin * 2);
                    string rightMargin = GetSeparator(margin.right);
                    string leftMargin = GetSeparator(margin.left);
                    string separator = $"{rem}(\"{GetSeparator(length)}\")\n";
                    result += separator;
                    result += rem;
                    result += "(\"";
                    result += leftMargin;
                    result += title;
                    result += rightMargin;
                    result += "\")\n";
                    result += separator;
                }
                result += "\n";
                {
                    string title = $"{endString}{item}";
                    (int right, int left) margin = GetMargin(title, max + minMargin * 2);
                    string rightMargin = GetSeparator(margin.right);
                    string leftMargin = GetSeparator(margin.left);
                    string separator = $"{rem}(\"{GetSeparator(length)}\")\n";
                    result += separator;
                    result += rem;
                    result += "(\"";
                    result += leftMargin;
                    result += title;
                    result += rightMargin;
                    result += "\")\n";
                    result += separator;
                }
                result += "\n\n";
            }
            return result;
        }
        private int GetLongestPossibleTitle(IEnumerable<string> collection, int longestAdditionalInfo)
        {
            int longestItem = 0;
            foreach (var item in collection)
            {
                longestItem = Math.Max(longestItem, item.Length);
            }
            return longestItem + longestAdditionalInfo;
        }
        private string GetSeparator(int length)
        {
            string s = "";
            for (int i = 0; i < length; i++)
            {
                s += "–";
            }
            return s;
        }
        private (int right, int left) GetMargin(string item, int length)
        {
            int right, left;

            left = minMargin;
            right = length - left - item.Length;

            //int len = length - item.Length;

            //if ((double)len % 2 == 0)
            //{
            //    right = left = len / 2;
            //}
            //else
            //{
            //    left = len / 2;
            //    right = left + 1;
            //}
            return (right, left);
        }
        private string GetStartString(Type type)
        {
            return $"Początek {TypeStringValuesDictionary.FirstOrDefault(x => x.Key == type).Value.name} dla ";
        }
        private string GetEndString(Type type)
        {
            return $"Koniec {TypeStringValuesDictionary.FirstOrDefault(x => x.Key == type).Value.name} dla ";
        }
        private void InsertEnumValuesIntoTypeList()
        {
            var types = Enum.GetNames(typeof(Type));
            foreach (var item in types)
            {
                typeList.Add(item);
            }
        }
    }
}
