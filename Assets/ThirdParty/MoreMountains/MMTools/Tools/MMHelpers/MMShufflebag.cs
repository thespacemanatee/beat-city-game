using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A class to use to get more controlled randomness, taking values out of the bag randomly, and never getting them
    ///     again.
    ///     Usage :
    ///     var shuffleBag = new ShuffleBag(40);
    ///     for (int i = 0; i
    ///     <40; i++)
    ///         {
    ///         newValue= something;
    ///         shuffleBag.Add( newValue, amount);
    ///         }
    ///         then :
    ///         float something= shuffleBag.Pick();
    /// </summary>
    public class MMShufflebag<T>
    {
        protected List<T> _contents;
        protected int _currentIndex = -1;
        protected T _currentItem;

        /// <summary>
        ///     Initializes the shufflebag
        /// </summary>
        /// <param name="initialCapacity"></param>
        public MMShufflebag(int initialCapacity)
        {
            _contents = new List<T>(initialCapacity);
        }

        public int Capacity => _contents.Capacity;
        public int Size => _contents.Count;

        /// <summary>
        ///     Adds the specified quantity of the item to the bag
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        public virtual void Add(T item, int quantity)
        {
            for (var i = 0; i < quantity; i++) _contents.Add(item);
            _currentIndex = Size - 1;
        }

        /// <summary>
        ///     Returns a random item from the bag
        /// </summary>
        /// <returns></returns>
        public T Pick()
        {
            if (_currentIndex < 1)
            {
                _currentIndex = Size - 1;
                _currentItem = _contents[0];
                return _currentItem;
            }

            var position = Random.Range(0, _currentIndex);

            _currentItem = _contents[position];
            _contents[position] = _contents[_currentIndex];
            _contents[_currentIndex] = _currentItem;
            _currentIndex--;

            return _currentItem;
        }
    }
}