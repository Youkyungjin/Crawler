using System.Collections;

//========================================================
// class BaseSingleton
//========================================================
// - for making singleton object
// - usage
//		+ declare class(derived )	
//			public class OnlyOne : BaseSingleton< OnlyOne >
//		+ client
//			OnlyOne.Instance.[method]
//========================================================
public abstract class BaseSingleton<T> where T : new()
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new T();
			}
			return instance;
		}
	}
}