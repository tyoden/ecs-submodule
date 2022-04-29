namespace ME.ECS {
    
    using Collections;

    #if ECS_COMPILE_IL2CPP_OPTIONS
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.NullChecks, false),
     Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false),
     Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    #endif
    public struct EntitiesIndexer {

        public struct KeyValuePair : System.IEquatable<KeyValuePair> {

            public int entityId;
            public int componentId;

            public KeyValuePair(int entityId, int componentId) {
                this.entityId = entityId;
                this.componentId = componentId;
            }

            public bool Equals(KeyValuePair other) {
                return this.entityId == other.entityId && this.componentId == other.componentId;
            }

            public override bool Equals(object obj) {
                return obj is KeyValuePair other && Equals(other);
            }

            public override int GetHashCode() {
                unchecked {
                    return (this.entityId * 397) ^ this.componentId;
                }
            }

        }
        
        [ME.ECS.Serializer.SerializeField]
        private HashSetCopyable<KeyValuePair> data;
        [ME.ECS.Serializer.SerializeField]
        private HashSetCopyable<long> index;

        public void Initialize(int capacity) {

            if (this.data == null) this.data = PoolHashSetCopyable<KeyValuePair>.Spawn(capacity);
            if (this.index == null) this.index = PoolHashSetCopyable<long>.Spawn(capacity);

        }

        public void Validate(int capacity) {

            if (this.data != null) this.data.SetCapacity(capacity);
            if (this.index != null) this.index.SetCapacity(capacity);
            
        }

        public bool Has(int entityId, int componentId) {

            var key = MathUtils.GetKey(entityId, componentId);
            return this.index.Contains(key);

        }

        public HashSetCopyable<KeyValuePair> Get() {

            return this.data;

        }

        public void Set(int entityId, int componentId) {

            var key = MathUtils.GetKey(entityId, componentId);
            if (this.index.Add(key) == true) {
                
                this.data.Add(new KeyValuePair(entityId, componentId));
                
            }

        }

        public void Remove(int entityId, int componentId) {
            
            var key = MathUtils.GetKey(entityId, componentId);
            if (this.index.Remove(key) == true) {

                this.data.RemoveWhere(entityId, (e, kv) => kv.entityId == e);
                
            }

        }

        public void CopyFrom(in EntitiesIndexer other) {
            
            ArrayUtils.Copy(other.data, ref this.data);
            ArrayUtils.Copy(other.index, ref this.index);
            
        }

        public void Recycle() {
            
            PoolHashSetCopyable<KeyValuePair>.Recycle(ref this.data);
            PoolHashSetCopyable<long>.Recycle(ref this.index);
            
        }

    }

}