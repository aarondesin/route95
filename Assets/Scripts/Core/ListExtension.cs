using System;
using System.Collections.Generic;

public static class ListExtension {

	public static T Head<T> (this List<T> list) {
		return list[0];
	}

	public static T Tail<T> (this List<T> list) {
		return list[list.Count-1];
	}
	
}

