using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;

namespace school_api.Data.Models
{
    public class AdminTransaction : Transaction
    {
        [Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never),
            Bindable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Guid UserId
        {
            get { return base.UserId; }
            set { base.UserId = value; }
        }
    }
}

