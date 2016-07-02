using System;

public static class ByteArrayEx
{
	public static string GetString(this byte[] bytes)
	{
		return System.Text.Encoding.UTF8.GetString (bytes);
	}
}

