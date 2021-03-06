﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Swashbuckle.Swagger.Annotations;

namespace FriGo.Api.Controllers
{
    [SwaggerResponseRemoveDefaults]
    public abstract class BaseFriGoController : ApiController
    {
        protected readonly IMapper AutoMapper;

        protected BaseFriGoController(IMapper autoMapper)
        {
            this.AutoMapper = autoMapper;
        }
    }
}
