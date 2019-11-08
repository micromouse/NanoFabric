using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanoFabric.Core.Exceptions;
using System.Collections.Generic;

namespace SampleService.Kestrel.Controllers {
    /// <summary>
    /// 测试API
    /// </summary>
    [Route("api/[controller]")]
    public class ValuesController : Controller {
        /// <summary>
        /// GET api/values
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get() {
            var ex = StandardException.Caused("001", "001号错误");
            ex.InnerError = StandardException.Caused("001-1", "001-1号错误");
            ex.Throw();
            return new[] { "value1", "value2", "myvalue" };
        }

        /// <summary>
        ///  GET api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize()]
        public string Get(int id) {
            return $"{id}";
        }

        /// <summary>
        ///  POST api/values
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public void Post([FromBody]string value) {
        }

        /// <summary>
        /// PUT api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value) {
        }

        /// <summary>
        ///  DELETE api/values/5
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}
