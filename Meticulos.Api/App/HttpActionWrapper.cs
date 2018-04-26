using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Meticulos.Api.App
{
    public static class FunctionWrapper
    {
        public static async Task<IActionResult> ExecuteFunction<T>(
            Controller controller, 
            Func<Task<T>> function)
        {
            try
            {
                var result = await function();
                return controller.Ok(result);
            }
            catch (ApplicationException ex)
            {
                return controller.StatusCode(409, ex.Message);
            }
            catch (Exception ex)
            {
                return controller.StatusCode(500, ex.Message);
            }
        }

        //public static async Task<IActionResult> ExecuteAction(
        //    Controller controller,
        //    Action action)
        //{
        //    try
        //    {
        //        await Task.Run(() => action());
        //        return controller.Ok();
        //    }
        //    catch (ApplicationException ex)
        //    {
        //        return controller.StatusCode(409, ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return controller.StatusCode(500, ex.Message);
        //    }
        //}

        public static async Task<IActionResult> ExecuteAction(
            Controller controller,
            Action action)
        {
            try
            {
                action();
                return controller.Ok();
            }
            catch (ApplicationException ex)
            {
                return controller.StatusCode(409, ex.Message);
            }
            catch (Exception ex)
            {
                return controller.StatusCode(500, ex.Message);
            }
        }
    }
}
