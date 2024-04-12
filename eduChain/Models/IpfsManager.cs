using Ipfs.Http;
namespace eduChain.Models;

public class IpfsManager
{
    private static readonly Lazy<IpfsManager> instance = new Lazy<IpfsManager>(() => new IpfsManager());

    private IpfsClient ipfs;

    public IpfsManager()
    {
        ipfs = new IpfsClient();
    }
    public static IpfsManager Instance => instance.Value;
    public async Task<bool> RetrieveFileFromIpfsAsync(string fileId, string savePath)
        {
            try
            {
                using (Stream fileStream = await ipfs.FileSystem.ReadFileAsync(fileId))
                {
                    // Read the file stream into a byte array
                    byte[] fileBytes = new byte[fileStream.Length];
                    await fileStream.ReadAsync(fileBytes, 0, fileBytes.Length);

                    // Write the byte array to a file
                    File.WriteAllBytes(savePath, fileBytes);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error retrieving file from IPFS: {ex.Message}");
                return false;
            }
        }
}
