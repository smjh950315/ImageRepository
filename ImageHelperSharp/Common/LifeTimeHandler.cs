namespace ImageHelperSharp.Common
{
    internal unsafe class LifeTimeHandler : IDisposable
    {
        void** m_handle_address;
        unsafe delegate*<void*, void> m_freeHandle;

        IntPtr* m_cs_handle_address;
        unsafe delegate*<IntPtr, void> m_cs_freeHandle;

        private bool disposedValue;

        unsafe public LifeTimeHandler(void** handle_address, delegate*<void*, void> callback_freeHandle)
        {
            this.m_handle_address = handle_address;
            this.m_freeHandle = callback_freeHandle;
        }

        unsafe public LifeTimeHandler(IntPtr* handle_address, delegate*<IntPtr, void> callback_freeHandle)
        {
            this.m_cs_handle_address = handle_address;
            this.m_cs_freeHandle = callback_freeHandle;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                unsafe
                {
                    if (this.m_handle_address != null && this.m_freeHandle != null)
                    {
                        if (*this.m_handle_address != null)
                        {
                            this.m_freeHandle(*this.m_handle_address);
                            *this.m_handle_address = null;
                        }
                    }
                    this.m_handle_address = null;
                    this.m_freeHandle = null;

                    if (this.m_cs_handle_address != null && this.m_cs_freeHandle != null)
                    {
                        if (*this.m_cs_handle_address != IntPtr.Zero)
                        {
                            this.m_cs_freeHandle(*this.m_cs_handle_address);
                            *this.m_cs_handle_address = IntPtr.Zero;
                        }
                    }
                    this.m_cs_handle_address = null;
                    this.m_cs_freeHandle = null;
                }

                this.disposedValue = true;
            }
        }

        ~LifeTimeHandler()
        {
            this.Dispose(disposing: false);
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
