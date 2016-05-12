using UnityEngine;
using System;
using System.Collections;

public class Map <T> where T: class {

	T[,] values;

	public Map (int width) {
		values = new T[width, width];
	}
		
	public void Resize () {
		int oldWidth = values.GetLength(0);
		T[,] newValues = new T[oldWidth*2,oldWidth*2];
		for (int x=0; x<oldWidth; x++) {
			for (int y=0; y<oldWidth; y++) {
				newValues[x+oldWidth/2, y+oldWidth/2] = values[x,y];
			}
		}
		values = newValues;
	}

	public T At (IntVector2 i) {
		return At (i.x, i.y);
	}

	public T At (int x, int y) {
		int w = Width;
		if (values == null || w == 0) return null;
		if (x+w/2 < 0 || x+w/2 >= w || 
			y+w/2 < 0 || y+w/2 >= w) return null;
		//Debug.Log("get "+x+" "+y);
		return values[x+w/2,y+w/2];
	}

	public void Set (int x, int y, T item) {
		try {
			//Debug.Log(x + " "+ y + " went to ");
			int width = Width;
			while (x +Width/2 >= Width || y+Width/2 >= Width ||
			x +Width/2 < 0 || y+Width/2 < 0) Resize();

			values[x+Width/2,y+Width/2] = item;
			//Debug.Log((x+Width/2) + " " +(y+Width/2)+" Width: "+Width);
			//Debug.Log("set "+x+" "+y);

		} catch (IndexOutOfRangeException e) {
			Debug.LogError("Map.Set(): inputs ("+x+","+y+") out of bounds! " + ToString() + " " + e.Message);
			return;
		}
	}

	public int Width {
		get {
			return values.GetLength(0);
		}
	}

	public string ToString () {
		return "Width: "+Width;
	}
}
