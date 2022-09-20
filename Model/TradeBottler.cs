﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualHFT.Model
{
    public class TradeBottler
    {
        public CloseExecution closeExecution;
        public OpenExecution openExecution;
        public string ProviderName;
        public string Symbol;
        public Execution execution
        {
            get
            {
                if (closeExecution != null)
                    return new Execution(closeExecution);
                else if (openExecution != null)
                    return new Execution(openExecution);
                else
                    return null;
            }
        }
    }
}
