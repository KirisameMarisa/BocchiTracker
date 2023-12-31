﻿using System;
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

        public void SetLoadFilename(string inFilename)
        {
            _configRepository.SetLoadFilename(inFilename);
        }

        public string GetLoadFilename()
        {
            return _configRepository.GetLoadFilename();
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

        public void Save(T inSettings)
        {
            _configRepository.Save(inSettings);
            _cachedConfig = inSettings;
            _isCacheValid = true;
        }
    }
}
