@@ -11,12 +11,11 @@ public class Riff {
		
	};

	List<Note> notes = new List<Note>(); // contains notes
	List<Note> notes = new List<Note>();
	 
	public bool pause = true; // if player is looping the riff or just want silent
	public MusicManager.Key currentKey = MusicManager.Key.EMajor;
	public static int drumRiffIndex = 0;

	public MusicManager.Key currentKey = MusicManager.Key.CMajor;
				
	void Sounds(Instrument currentInstrument, MusicManager.Key currentKey){
		switch (currentInstrument) {
		case Instrument.Drums:
@@ -41,17 +40,9 @@ public class Riff {



	public void playriff(int pos){ // plays all the notes within the sequencer aka the riff 
		notes[drumRiffIndex].Play(pos);
	public void playriff(){
	
	}

	
	/*public void Play (int pos) {
		foreach (Instrument hit in riff[pos]) {
			MusicManager.PlayPercussion (hit);
		}
	}*/
	
	public static void checkPlay(bool select){
		if (select) {