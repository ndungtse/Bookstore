
namespace BookStoreApi.Utils;

public class ApiResponse<Model> {
    private Model data;
    private string message;
    private bool success;

    public ApiResponse(string message, Model data, bool success){
        this.data = data;
        this.message = message;
        this.success = success;
    }
}