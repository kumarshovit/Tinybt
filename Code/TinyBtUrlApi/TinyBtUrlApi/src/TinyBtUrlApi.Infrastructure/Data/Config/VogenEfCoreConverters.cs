using TinyBtUrlApi.Core.ContributorAggregate;
using Vogen;

namespace TinyBtUrlApi.Infrastructure.Data.Config;

[EfCoreConverter<ContributorId>]
[EfCoreConverter<ContributorName>]
internal partial class VogenEfCoreConverters;
