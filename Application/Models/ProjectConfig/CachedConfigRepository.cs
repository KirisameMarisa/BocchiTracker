using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Config
{
    public class CachedConfigRepository<T>
    {
        private ConfigRepository<T> _configRepository;
        private T? _cachedConfig;
        private bool _isCacheValid;

        public CachedConfigRepository(ConfigRepository<T> inConfigRepository)
        {
            _configRepository = inConfigRepository;
            _isCacheValid = false;
        }

        public T? Load()
        {
            if (_isCacheValid)
            {
                return _cachedConfig;
            }

            T? outConfig;
            bool loadSuccess = _configRepository.TryLoad(out outConfig);

            if (loadSuccess && outConfig != null)
            {
                _cachedConfig = outConfig;
                _isCacheValid = true;
            }
            return _cachedConfig;
        }

        public void Save(T settings)
        {
            _configRepository.Save(settings);
            _cachedConfig = settings;
            _isCacheValid = true;
        }
    }
}
