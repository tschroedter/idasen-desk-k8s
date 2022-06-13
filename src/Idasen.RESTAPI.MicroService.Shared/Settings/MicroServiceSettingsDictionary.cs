using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Serilog ;

namespace Idasen.RESTAPI.MicroService.Shared.Settings ;

public class MicroServiceSettingsDictionary
    : Dictionary < string , MicroServiceSettings > ,
      IMicroServiceSettingsDictionary
{
    private readonly ILogger _logger ;

    public MicroServiceSettingsDictionary ( ILogger logger )
    {
        _logger = logger ;
    }

    public IMicroServiceSettingsDictionary Initialize ( IList < MicroServiceSettings > settings )
    {
        Clear ( ) ;

        foreach ( var setting in settings )
        {
            if ( ! TryAdd ( setting.Path.ToLower ( ) ,
                            setting ) )
                _logger.Warning ( $"Duplicated URL key '{setting.Path}' value is ignored ('{setting}')." ) ;
        }

        _logger.Debug ( $"{nameof(MicroServiceSettingsDictionary)}" );

        foreach ( var key in Keys )
        {
            _logger.Debug ( $"{key}: {this[key]}" );
        }

        return this ;
    }
}