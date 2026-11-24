using System;
using System.Linq;
using System.Threading;

using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Server.Migrations.Routines;

/// <summary>
/// Remove duplicate playlist entries.
/// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
[JellyfinMigration("2025-04-20T19:00:00", nameof(RemoveDuplicatePlaylistChildren), "96C156A2-7A13-4B3B-A8B8-FB80C94D20C0")]
internal class RemoveDuplicatePlaylistChildren : IMigrationRoutine
#pragma warning restore CS0618 // Type or member is obsolete
{
    private readonly ILogger<RemoveDuplicatePlaylistChildren> _logger;
    private readonly ILibraryManager _libraryManager;
    private readonly IPlaylistManager _playlistManager;

    public RemoveDuplicatePlaylistChildren(
        ILogger<RemoveDuplicatePlaylistChildren> logger,
        ILibraryManager libraryManager,
        IPlaylistManager playlistManager)
    {
        _logger = logger;
        _libraryManager = libraryManager;
        _playlistManager = playlistManager;
    }

    /// <summary>
    /// Gets the identifier of the migration routine.
    /// </summary>
    public Guid Id => Guid.Parse("{96C156A2-7A13-4B3B-A8B8-FB80C94D20C0}");

    /// <summary>
    /// Gets the name of the migration routine.
    /// </summary>
    public string Name => "RemoveDuplicatePlaylistChildren";

    /// <summary>
    /// Gets a value indicating whether the migration routine is performed on a new installation.
    /// </summary>
    public bool PerformOnNewInstall => false;

    /// <inheritdoc/>
    public void Perform()
    {
        var playlists = _libraryManager.GetItemList(new InternalItemsQuery
        {
            IncludeItemTypes = [BaseItemKind.Playlist]
        })
        .Cast<Playlist>()
        .Where(p => !p.OpenAccess || !p.OwnerUserId.Equals(Guid.Empty))
        .ToArray();

        if (playlists.Length > 0)
        {
            foreach (var playlist in playlists)
            {
                var linkedChildren = playlist.LinkedChildren;
                if (linkedChildren.Length > 0)
                {
                    var nullItemChildren = linkedChildren.Where(c => c.ItemId is null);
                    var deduplicatedChildren = linkedChildren
                        .Where(c => c.ItemId is not null)
                        .DistinctBy(c => c.ItemId);
                    var newLinkedChildren = nullItemChildren.Concat(deduplicatedChildren).ToArray();
                    playlist.LinkedChildren = newLinkedChildren;
                    playlist.UpdateToRepositoryAsync(ItemUpdateType.MetadataEdit, CancellationToken.None).GetAwaiter().GetResult();
                    _playlistManager.SavePlaylistFile(playlist);
                }
            }
        }
    }
}
