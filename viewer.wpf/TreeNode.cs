using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Viewer.Wpf
{
    public interface ITreeNode
    {
        int ID { get; }

        AggregatedMoney Money { get; }

        string Title { get; }

        ICollection<ITreeNode> SubCollection { get; }

        int Level { get; }
    }

    internal class InnerNode : ITreeNode
    {
        public int ID { get; set; }

        public AggregatedMoney Money
        {
            get
            {
                var money = new AggregatedMoney();
                foreach (var node in SubCollection)
                {
                    money.Add(node.Money);
                }

                return money;
            }
        }

        public string Title { get; set; } = string.Empty;

        public ICollection<ITreeNode> SubCollection { get; } = new ObservableCollection<ITreeNode>();

        public int Level { get; set; }
    }

    internal class EndNode : ITreeNode
    {
        public EndNode(AggregatedMoney money, string title)
        {
            Money = money;
            Title = title;
        }

        public int ID { get; }

        public AggregatedMoney Money { get; }

        public string Title { get; }

        public ICollection<ITreeNode> SubCollection { get; } = new List<ITreeNode>();

        public int Level { get; } = 3;
    }
}
