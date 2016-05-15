using UnityEngine;
using System;
using System.Collections;

public class Map <T> where T: class {

	T[,] values;
	int width;

	public Map (int w) {
		values = new T[w, w];
		width = w;
	}
		
	public void Resize () {
		if (width == 0)
			width = 1;
		int oldWidth = width;
		T[,] newValues = new T[oldWidth*2,oldWidth*2];
		for (int x=0; x<oldWidth; x++) {
			for (int y=0; y<oldWidth; y++) {
				newValues[x+oldWidth/2, y+oldWidth/2] = values[x,y];
			}
		}
		values = newValues;
		width *= 2;
	}

	public T At (IntVector2 i) {
		return At (i.x, i.y);
	}

	public T At (int x, int y) {
		if (values == null || width == 0) return null;
		if (x+width/2 < 0 || x+width/2 >= width || 
			y+width/2 < 0 || y+width/2 >= width) return null;
		//Debug.Log("get "+x+" "+y);
		return values[x+width/2,y+width/2];
	}

	public void Set (int x, int y, T item) {
		try {
			//Debug.Log("before resize "+width);
			//Debug.Log(x + " "+ y + " went to ");
			while (x +width/2 >= width || y+width/2 >= width ||
			x +width/2 < 0 || y+width/2 < 0) Resize();

			values[x+width/2,y+width/2] = item;
			//Debug.Log((x+Width/2) + " " +(y+Width/2)+" Width: "+Width);
			//Debug.Log("set "+x+" "+y);

			//Debug.Log("after resize "+width);

		} catch (IndexOutOfRangeException e) {
			Debug.LogError("Map.Set(): inputs ("+x+","+y+") out of bounds! " + ToString() + " " + e.Message);
			return;
		}
	}

	public int Width {
		get {
			return width;
		}
	}

	public override string ToString () {
		return "Width: "+width;
	}
}
