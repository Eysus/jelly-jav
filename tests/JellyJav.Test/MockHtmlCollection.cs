using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Common;
using AngleSharp.Dom;

namespace JellyJav.Test
{
    internal sealed class MockHtmlCollection<T> : IHtmlCollection<T> where T : class, IElement
    {
        private readonly IEnumerable<T> _elements;

        public MockHtmlCollection(IEnumerable<T> elements)
        {
            _elements = elements;
        }

        public T this[Int32 index] => _elements.GetItemByIndex(index);

        public T? this[String id] => _elements.GetElementById(id);

        public Int32 Length => _elements.Count();

        public IEnumerator<T> GetEnumerator() => _elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();
    }
}
