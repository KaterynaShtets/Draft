using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/*
CREATE TABLE [dbo].[Reading] (
    [Id]       INT            NOT NULL IDENTITY,
    [LecId]    INT            NOT NULL,
    [UserName] NVARCHAR (256) NOT NULL,
    [Begin]    DATETIME       NOT NULL,
    [End]      DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
*/
namespace Tut20.Models
{
    [Table("Reading")]
    public class Reading
    {
        public int Id { get; set; }
        public int LecId { get; set; }
        public string UserName { get; set; }
        public DateTime Begin { get; set; }
        public DateTime? End { get; set; }
    }
    public class ReadingComparer : IEqualityComparer<Reading>
    {
        public bool Equals(Reading x, Reading y)
        {
            if (x.LecId == y.LecId
                    && x.UserName == y.UserName)
                return true;

            return false;
        }

        public int GetHashCode(Reading obj)
        {
            return obj.LecId.GetHashCode();
        }

    }
    public class ReadingComparerName : IEqualityComparer<Reading>
    {
        public bool Equals(Reading x, Reading y)
        {
            if ( x.UserName == y.UserName)
                return true;

            return false;
        }

        public int GetHashCode(Reading obj)
        {
            return obj.UserName.GetHashCode();
        }

    }
}
