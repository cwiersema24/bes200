using Microsoft.AspNetCore.Mvc;

public static class ControllerExtensions
{
    public static ActionResult Maybe<T>(this ControllerBase c, T obj)
    {
        if (obj == null)
        {
            return new NotFoundResult();
        }
        else
        {
            return new OkObjectResult(obj);
        }
    }
}
