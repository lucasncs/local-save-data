namespace Seven.SaveSystem
{
	public interface ICryptographer
	{
		string Decrypt(byte[] soup, string key);
		byte[] Encrypt(string original, string key);
	}
}