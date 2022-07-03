﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace MoreMountains.Tools
{
    [Serializable]
    /// <summary>
    /// Multiple object pooler object.
    /// </summary>
    public class MMMultipleObjectPoolerObject
    {
        public GameObject GameObjectToPool;
        public int PoolSize;
        public bool PoolCanExpand = true;
        public bool Enabled = true;
    }

    /// <summary>
    ///     The various methods you can pull objects from the pool with
    /// </summary>
    public enum MMPoolingMethods
    {
        OriginalOrder,
        OriginalOrderSequential,
        RandomBetweenObjects,
        RandomPoolSizeBased
    }

    /// <summary>
    ///     This class allows you to have a pool of various objects to pool from.
    /// </summary>
    [AddComponentMenu("More Mountains/Tools/Object Pool/MMMultipleObjectPooler")]
    public class MMMultipleObjectPooler : MMObjectPooler
    {
        /// the list of objects to pool
        public List<MMMultipleObjectPoolerObject> Pool;

        [MMInformation(
            "A MultipleObjectPooler is a reserve of objects, to be used by a Spawner. When asked, it will return an object from the pool (ideally an inactive one) chosen based on the pooling method you've chosen.\n- OriginalOrder will spawn objects in the order you've set them in the inspector (from top to bottom)\n- OriginalOrderSequential will do the same, but will empty each pool before moving to the next object\n- RandomBetweenObjects will pick one object from the pool, at random, but ignoring its pool size, each object has equal chances to get picked\n- PoolSizeBased randomly choses one object from the pool, based on its pool size probability (the larger the pool size, the higher the chances it'll get picked)'...",
            MMInformationAttribute.InformationType.Info, false)]
        /// the chosen pooling method
        public MMPoolingMethods PoolingMethod = MMPoolingMethods.RandomPoolSizeBased;

        [MMInformation(
            "If you set CanPoolSameObjectTwice to false, the Pooler will try to prevent the same object from being pooled twice to avoid repetition. This will only affect random pooling methods, not ordered pooling.",
            MMInformationAttribute.InformationType.Info, false)]
        /// whether or not the same object can be pooled twice in a row. If you set CanPoolSameObjectTwice to false, the Pooler will try to prevent the same object from being pooled twice to avoid repetition. This will only affect random pooling methods, not ordered pooling.
        public bool CanPoolSameObjectTwice = true;

        /// a unique name that should match on all MMMultipleObjectPoolers you want to use together
        [MMCondition("MutualizeWaitingPools", true)]
        public string MutualizedPoolName = "";

        protected int _currentIndex;
        protected int _currentIndexCounter;

        /// the actual object pool
        protected GameObject _lastPooledObject;

        /// <summary>
        ///     Determines the name of the object pool.
        /// </summary>
        /// <returns>The object pool name.</returns>
        protected override string DetermineObjectPoolName()
        {
            if (MutualizedPoolName == null || MutualizedPoolName == "")
                return "[MultipleObjectPooler] " + name;
            return "[MultipleObjectPooler] " + MutualizedPoolName;
        }

        /// <summary>
        ///     Fills the object pool with the amount of objects you specified in the inspector.
        /// </summary>
        public override void FillObjectPool()
        {
            if (Pool == null || Pool.Count == 0) return;

            // we create a waiting pool, if one already exists, no need to fill anything
            if (!CreateWaitingPool()) return;

            // if there's only one item in the Pool, we force CanPoolSameObjectTwice to true
            if (Pool.Count <= 1) CanPoolSameObjectTwice = true;

            bool stillObjectsToPool;
            int[] poolSizes;

            // if we're gonna pool in the original inspector order
            switch (PoolingMethod)
            {
                case MMPoolingMethods.OriginalOrder:
                    stillObjectsToPool = true;
                    // we store our poolsizes in a temp array so it doesn't impact the inspector
                    poolSizes = new int[Pool.Count];
                    for (var i = 0; i < Pool.Count; i++) poolSizes[i] = Pool[i].PoolSize;

                    // we go through our objects in the order they were in the inspector, and fill the pool while we find objects to add
                    while (stillObjectsToPool)
                    {
                        stillObjectsToPool = false;
                        for (var i = 0; i < Pool.Count; i++)
                            if (poolSizes[i] > 0)
                            {
                                AddOneObjectToThePool(Pool[i].GameObjectToPool);
                                poolSizes[i]--;
                                stillObjectsToPool = true;
                            }
                    }

                    break;
                case MMPoolingMethods.OriginalOrderSequential:
                    // we store our poolsizes in a temp array so it doesn't impact the inspector
                    foreach (var pooledGameObject in Pool)
                        for (var i = 0; i < pooledGameObject.PoolSize; i++)
                            AddOneObjectToThePool(pooledGameObject.GameObjectToPool);
                    break;
                default:
                    var k = 0;
                    // for each type of object specified in the inspector
                    foreach (var pooledGameObject in Pool)
                    {
                        // if there's no specified number of objects to pool for that type of object, we do nothing and exit
                        if (k > Pool.Count) return;

                        // we add, one by one, the number of objects of that type, as specified in the inspector
                        for (var j = 0; j < Pool[k].PoolSize; j++)
                            AddOneObjectToThePool(pooledGameObject.GameObjectToPool);
                        k++;
                    }

                    break;
            }
        }

        /// <summary>
        ///     Adds one object of the specified type to the object pool.
        /// </summary>
        /// <returns>The object that just got added.</returns>
        /// <param name="typeOfObject">The type of object to add to the pool.</param>
        protected virtual GameObject AddOneObjectToThePool(GameObject typeOfObject)
        {
            var newGameObject = Instantiate(typeOfObject);
            SceneManager.MoveGameObjectToScene(newGameObject, gameObject.scene);
            newGameObject.gameObject.SetActive(false);
            if (NestWaitingPool) newGameObject.transform.SetParent(_waitingPool.transform);
            newGameObject.name = typeOfObject.name;
            _objectPool.PooledGameObjects.Add(newGameObject);
            return newGameObject;
        }

        /// <summary>
        ///     Gets a random object from the pool.
        /// </summary>
        /// <returns>The pooled game object.</returns>
        public override GameObject GetPooledGameObject()
        {
            GameObject pooledGameObject;
            switch (PoolingMethod)
            {
                case MMPoolingMethods.OriginalOrder:
                    pooledGameObject = GetPooledGameObjectOriginalOrder();
                    break;
                case MMPoolingMethods.RandomPoolSizeBased:
                    pooledGameObject = GetPooledGameObjectPoolSizeBased();
                    break;
                case MMPoolingMethods.RandomBetweenObjects:
                    pooledGameObject = GetPooledGameObjectRandomBetweenObjects();
                    break;
                case MMPoolingMethods.OriginalOrderSequential:
                    pooledGameObject = GetPooledGameObjectOriginalOrderSequential();
                    break;
                default:
                    pooledGameObject = null;
                    break;
            }

            if (pooledGameObject != null)
                _lastPooledObject = pooledGameObject;
            else
                _lastPooledObject = null;
            return pooledGameObject;
        }

        /// <summary>
        ///     Tries to find a gameobject in the pool according to the order the list has been setup in (one of each, no matter
        ///     how big their respective pool sizes)
        /// </summary>
        /// <returns>The pooled game object original order.</returns>
        protected virtual GameObject GetPooledGameObjectOriginalOrder()
        {
            int newIndex;
            // if we've reached the end of our list, we start again from the beginning
            if (_currentIndexCounter >= Pool[_currentIndex].PoolSize)
            {
                _currentIndexCounter = 0;
                _currentIndex++;
            }

            if (_currentIndex >= Pool.Count) ResetCurrentIndex();

            var searchedObject = GetPoolObject(Pool[_currentIndex].GameObjectToPool);

            if (_currentIndex >= _objectPool.PooledGameObjects.Count) return null;
            if (!searchedObject.Enabled)
            {
                _currentIndex++;
                return null;
            }

            // if the object is already active, we need to find another one
            if (_objectPool.PooledGameObjects[_currentIndex].gameObject.activeInHierarchy)
            {
                var findObject = FindInactiveObject(_objectPool.PooledGameObjects[_currentIndex].gameObject.name,
                    _objectPool.PooledGameObjects);
                if (findObject != null)
                {
                    _currentIndexCounter++;
                    return findObject;
                }

                // if its pool can expand, we create a new one
                if (searchedObject.PoolCanExpand)
                {
                    _currentIndexCounter++;
                    return AddOneObjectToThePool(searchedObject.GameObjectToPool);
                }

                // if it can't expand we return nothing
                return null;
            }

            // if the object is inactive, we return it
            newIndex = _currentIndex;
            _currentIndexCounter++;
            return _objectPool.PooledGameObjects[newIndex];
        }

        /// <summary>
        ///     Tries to find a gameobject in the pool according to the order the list has been setup in (one of each, no matter
        ///     how big their respective pool sizes)
        /// </summary>
        /// <returns>The pooled game object original order.</returns>
        protected virtual GameObject GetPooledGameObjectOriginalOrderSequential()
        {
            int newIndex;

            // if we've reached the end of our list, we start again from the beginning
            if (_currentIndex >= Pool.Count) ResetCurrentIndex();

            var searchedObject = GetPoolObject(Pool[_currentIndex].GameObjectToPool);

            if (_currentIndex >= _objectPool.PooledGameObjects.Count) return null;
            if (!searchedObject.Enabled)
            {
                _currentIndex++;
                return null;
            }

            // if the object is already active, we need to find another one
            if (_objectPool.PooledGameObjects[_currentIndex].gameObject.activeInHierarchy)
            {
                var findObject = FindInactiveObject(_objectPool.PooledGameObjects[_currentIndex].gameObject.name,
                    _objectPool.PooledGameObjects);
                if (findObject != null)
                {
                    _currentIndex++;
                    return findObject;
                }

                // if its pool can expand, we create a new one
                if (searchedObject.PoolCanExpand)
                {
                    _currentIndex++;
                    return AddOneObjectToThePool(searchedObject.GameObjectToPool);
                }

                // if it can't expand we return nothing
                return null;
            }

            // if the object is inactive, we return it
            newIndex = _currentIndex;
            _currentIndex++;
            return _objectPool.PooledGameObjects[newIndex];
        }

        /// <summary>
        ///     Randomly choses one object from the pool, based on its pool size probability (the larger the pool size, the higher
        ///     the chances it'll get picked)
        /// </summary>
        /// <returns>The pooled game object pool size based.</returns>
        protected virtual GameObject GetPooledGameObjectPoolSizeBased()
        {
            // we get a random index 
            var randomIndex = Random.Range(0, _objectPool.PooledGameObjects.Count);

            var overflowCounter = 0;

            // we check to see if that object is enabled, if it's not we loop
            while (!PoolObjectEnabled(_objectPool.PooledGameObjects[randomIndex]) &&
                   overflowCounter < _objectPool.PooledGameObjects.Count)
            {
                randomIndex = Random.Range(0, _objectPool.PooledGameObjects.Count);
                overflowCounter++;
            }

            if (!PoolObjectEnabled(_objectPool.PooledGameObjects[randomIndex])) return null;

            // if we can't pool the same object twice, we'll loop for a while to try and get another one
            overflowCounter = 0;
            while (!CanPoolSameObjectTwice
                   && _objectPool.PooledGameObjects[randomIndex] == _lastPooledObject
                   && overflowCounter < _objectPool.PooledGameObjects.Count)
            {
                randomIndex = Random.Range(0, _objectPool.PooledGameObjects.Count);
                overflowCounter++;
            }

            //  if the item we've picked is active
            if (_objectPool.PooledGameObjects[randomIndex].gameObject.activeInHierarchy)
            {
                // we try to find another inactive object of the same type
                var pulledObject = FindInactiveObject(_objectPool.PooledGameObjects[randomIndex].gameObject.name,
                    _objectPool.PooledGameObjects);
                if (pulledObject != null) return pulledObject;

                // if we couldn't find an inactive object of this type, we see if it can expand
                var searchedObject = GetPoolObject(_objectPool.PooledGameObjects[randomIndex].gameObject);
                if (searchedObject == null) return null;
                // if the pool for this object is allowed to grow (this is set in the inspector if you're wondering)
                if (searchedObject.PoolCanExpand)
                    return AddOneObjectToThePool(searchedObject.GameObjectToPool);
                // if it's not allowed to grow, we return nothing.
                return null;
            }

            // if the pool wasn't empty, we return the random object we've found.
            return _objectPool.PooledGameObjects[randomIndex];
        }

        /// <summary>
        ///     Gets one object from the pool, at random, but ignoring its pool size, each object has equal chances to get picked
        /// </summary>
        /// <returns>The pooled game object random between objects.</returns>
        protected virtual GameObject GetPooledGameObjectRandomBetweenObjects()
        {
            // we pick one of the objects in the original pool at random
            var randomIndex = Random.Range(0, Pool.Count);

            var overflowCounter = 0;

            // if we can't pool the same object twice, we'll loop for a while to try and get another one
            while (!CanPoolSameObjectTwice && Pool[randomIndex].GameObjectToPool == _lastPooledObject &&
                   overflowCounter < _objectPool.PooledGameObjects.Count)
            {
                randomIndex = Random.Range(0, Pool.Count);
                overflowCounter++;
            }

            var originalRandomIndex = randomIndex + 1;

            var objectFound = false;

            // while we haven't found an object to return, and while we haven't gone through all the different object types, we keep going
            overflowCounter = 0;
            while (!objectFound
                   && randomIndex != originalRandomIndex
                   && overflowCounter < _objectPool.PooledGameObjects.Count)
            {
                // if our index is at the end, we reset it
                if (randomIndex >= Pool.Count) randomIndex = 0;

                if (!Pool[randomIndex].Enabled)
                {
                    randomIndex++;
                    overflowCounter++;
                    continue;
                }

                // we try to find an inactive object of that type in the pool
                var newGameObject = FindInactiveObject(Pool[randomIndex].GameObjectToPool.name,
                    _objectPool.PooledGameObjects);
                if (newGameObject != null)
                {
                    objectFound = true;
                    return newGameObject;
                }

                // if there's none and if we can expand, we expand
                if (Pool[randomIndex].PoolCanExpand) return AddOneObjectToThePool(Pool[randomIndex].GameObjectToPool);
                randomIndex++;
                overflowCounter++;
            }

            return null;
        }

        /// <summary>
        ///     Gets an object of the specified name from the pool
        /// </summary>
        /// <returns>The pooled game object of type.</returns>
        /// <param name="type">Type.</param>
        protected virtual GameObject GetPooledGameObjectOfType(string searchedName)
        {
            var newObject = FindInactiveObject(searchedName, _objectPool.PooledGameObjects);

            if (newObject != null) return newObject;

            // if we've not returned the object, that means the pool is empty (at least it means it doesn't contain any object of that specific type)
            // so if the pool is allowed to expand
            var searchedObject = FindObject(searchedName, _objectPool.PooledGameObjects);
            if (searchedObject == null) return null;

            if (GetPoolObject(FindObject(searchedName, _objectPool.PooledGameObjects)).PoolCanExpand)
            {
                // we create a new game object of that type, we add it to the pool for further use, and return it.
                var newGameObject = Instantiate(searchedObject);
                SceneManager.MoveGameObjectToScene(newGameObject, gameObject.scene);
                _objectPool.PooledGameObjects.Add(newGameObject);
                return newGameObject;
            }

            // if the pool was empty for that object and not allowed to expand, we return nothing.
            return null;
        }

        /// <summary>
        ///     Finds an inactive object in the pool based on its name.
        ///     Returns null if no inactive object by that name were found in the pool
        /// </summary>
        /// <returns>The inactive object.</returns>
        /// <param name="searchedName">Searched name.</param>
        protected virtual GameObject FindInactiveObject(string searchedName, List<GameObject> list)
        {
            for (var i = 0; i < list.Count; i++)
                // if we find an object inside the pool that matches the asked type
                if (list[i].name.Equals(searchedName))
                    // and if that object is inactive right now
                    if (!list[i].gameObject.activeInHierarchy)
                        // we return it
                        return list[i];
            return null;
        }

        protected virtual GameObject FindAnyInactiveObject(List<GameObject> list)
        {
            for (var i = 0; i < list.Count; i++)
                // and if that object is inactive right now
                if (!list[i].gameObject.activeInHierarchy)
                    // we return it
                    return list[i];
            return null;
        }

        /// <summary>
        ///     Finds an object in the pool based on its name, active or inactive
        ///     Returns null if there's no object by that name in the pool
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="searchedName">Searched name.</param>
        protected virtual GameObject FindObject(string searchedName, List<GameObject> list)
        {
            for (var i = 0; i < list.Count; i++)
                // if we find an object inside the pool that matches the asked type
                if (list[i].name.Equals(searchedName))
                    // and if that object is inactive right now
                    return list[i];
            return null;
        }

        /// <summary>
        ///     Returns (if it exists) the MultipleObjectPoolerObject from the original Pool based on a GameObject.
        ///     Note that this is name based.
        /// </summary>
        /// <returns>The pool object.</returns>
        /// <param name="testedObject">Tested object.</param>
        protected virtual MMMultipleObjectPoolerObject GetPoolObject(GameObject testedObject)
        {
            if (testedObject == null) return null;
            var i = 0;
            foreach (var poolerObject in Pool)
            {
                if (testedObject.name.Equals(poolerObject.GameObjectToPool.name)) return poolerObject;
                i++;
            }

            return null;
        }

        protected virtual bool PoolObjectEnabled(GameObject testedObject)
        {
            var searchedObject = GetPoolObject(testedObject);
            if (searchedObject != null)
                return searchedObject.Enabled;
            return false;
        }

        public virtual void EnableObjects(string name, bool newStatus)
        {
            foreach (var poolerObject in Pool)
                if (name.Equals(poolerObject.GameObjectToPool.name))
                    poolerObject.Enabled = newStatus;
        }

        public virtual void ResetCurrentIndex()
        {
            _currentIndex = 0;
            _currentIndexCounter = 0;
        }
    }
}