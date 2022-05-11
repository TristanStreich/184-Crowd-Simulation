using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : HeapItem<T> {
	
	T[] items;
	int count;
	
	public Heap(int maxHeapSize) {
		items = new T[maxHeapSize];
	}
	
	public void Add(T item) {
		item.HeapIndex = count;
		items[count] = item;
		SortUp(item);
		count++;
	}

	public T RemoveFirst() {
		T firstItem = items[0];
		count--;
		items[0] = items[count];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}

	public void UpdateItem(T item) {
		SortUp(item);
	}

	public int Count {
		get {
			return count;
		}
	}

	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

    void Swap(T A, T B) {
		items[A.HeapIndex] = B;
		items[B.HeapIndex] = A;
		int AIndex = A.HeapIndex;
		A.HeapIndex = B.HeapIndex;
		B.HeapIndex = AIndex;
	}

	void SortUp(T item) {
		int indexParent = (item.HeapIndex-1)/2;
		while (true) {
			T parentItem = items[indexParent];
			if (item.CompareTo(parentItem) > 0) 
				Swap (item,parentItem);
			else 
				break;
			indexParent = (item.HeapIndex-1)/2;
		}
	}

	void SortDown(T item) {
		while (true) {
			int indexLeft = item.HeapIndex * 2 + 1, indexRight = item.HeapIndex * 2 + 2, swap = 0;
			if (indexLeft < count) {
				swap = indexLeft;
				if ((indexRight < count) && (items[indexLeft].CompareTo(items[indexRight]) < 0) )
						swap = indexRight;
				if (item.CompareTo(items[swap]) < 0) 
					Swap (item,items[swap]);
				else	
                    return;
			}
            else
			    return;
		}
	}
    
}

public interface HeapItem<T> : IComparable<T>{
    int HeapIndex{
        get;
        set;
    }
}
