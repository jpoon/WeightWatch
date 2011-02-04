namespace WeightWatch.Classes
{
    using System.Collections.Generic;

    public class LongListSelectorGroup<T> : IEnumerable<T>
    {
        public LongListSelectorGroup(string date, IEnumerable<T> items)
        {
            this.GroupHeader = date;
            this.GroupItem = date;
            this.Items = new List<T>(items);
        }

        public string GroupHeader
        {
            get;
            set;
        }

        public string GroupItem
        {
            get;
            set;
        }

        public IList<T> Items
        {
            get;
            set;
        }

        #region IEnumerable Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            LongListSelectorGroup<T> that = obj as LongListSelectorGroup<T>;
            return (that != null) && (this.GroupHeader.Equals(that.GroupHeader));
        }

        #endregion
    }
}
