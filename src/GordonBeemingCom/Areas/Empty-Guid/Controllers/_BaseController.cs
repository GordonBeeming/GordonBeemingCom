
namespace GordonBeemingCom.Areas.Empty_Guid.Controllers;

[Area(EmptyGuidArea)]
[Route(EmptyGuidArea)]
public abstract class BaseController : Controller
{
  private readonly ILogger _logger;

  public BaseController(ILogger logger)
  {
    _logger = logger;
  }
}
