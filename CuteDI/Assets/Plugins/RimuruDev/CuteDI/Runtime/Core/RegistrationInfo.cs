using System;

namespace AbyssMoth.CuteDI
{
    public readonly struct RegistrationInfo
    {
        public readonly string Tag;
        public readonly Type ServiceType;
        public readonly bool IsSingleton;

        public RegistrationInfo(string tag, Type serviceType, bool isSingleton)
        {
            Tag = tag;
            ServiceType = serviceType;
            IsSingleton = isSingleton;
        }
    }
    
    public readonly struct BindingHint
    {
        public readonly Type Service;
        public readonly Type Implementation;
        public readonly string Tag;
        public readonly bool Singleton;

        public BindingHint(Type service, Type implementation, string tag, bool singleton)
        {
            Service = service;
            Implementation = implementation;
            Tag = tag;
            Singleton = singleton;
        }
    }
    
    public readonly struct DebugEntry
    {
        public readonly string Tag;
        public readonly Type ServiceType;
        public readonly bool IsSingleton;
        public readonly int Depth;
        public readonly string ContainerName;

        public DebugEntry(string tag, Type serviceType, bool isSingleton, int depth, string containerName)
        {
            Tag = tag;
            ServiceType = serviceType;
            IsSingleton = isSingleton;
            Depth = depth;
            ContainerName = containerName;
        }
    }

}