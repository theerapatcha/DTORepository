
using AutoMapper;
using DTORepository.Common;
using DTORepository.Models;
using DTORepositoryTest.Samples.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepositoryTest.Samples.Dtos
{
    class AuthorDto : DtoBase<Author, AuthorDto>
    {

        public int Id { get; set; }
        public string Name { get; set; }

    }

}
