using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoilSimulater.Utilities
{
    public class CancelToken
    {
        private Boolean m_IsCanceld = false;

        public Boolean IsCanceled
        {
            get { return m_IsCanceld; }
        }

        public void Cancel()
        {
            m_IsCanceld = true;

            if (Canceled != null)
                Canceled(this, new EventArgs());
        }

        public event EventHandler Canceled;
    }
}
