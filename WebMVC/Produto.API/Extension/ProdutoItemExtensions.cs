using Produto.API.Model;

namespace Produto.API.Extension
{
    public static class ProdutoItemExtensions
    {
        public static void FillProductUrl(this ProdutoItem item, string picBaseUrl, bool azureStorageEnabled)
        {
            if (item != null)
            {
                item.PictureUri = azureStorageEnabled
                   ? picBaseUrl + item.PictureFileName
                   : picBaseUrl.Replace("[0]", item.Id.ToString());
            }
        }
    }
}
