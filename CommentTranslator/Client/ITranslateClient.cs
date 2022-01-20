using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework;

namespace CommentTranlsator.Client
{
    public interface ITranslateClient
    {
        Task<IAPIResponse> Translate(string text);
    }
}
