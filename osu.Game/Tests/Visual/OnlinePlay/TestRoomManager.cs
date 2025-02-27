// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osu.Game.Rulesets;
using osu.Game.Screens.OnlinePlay.Components;

namespace osu.Game.Tests.Visual.OnlinePlay
{
    /// <summary>
    /// A very simple <see cref="RoomManager"/> for use in online play test scenes.
    /// </summary>
    public partial class TestRoomManager : RoomManager
    {
        public Action<Room, string?>? JoinRoomRequested;

        private int currentRoomId;

        public override void JoinRoom(Room room, string? password = null, Action<Room>? onSuccess = null, Action<string>? onError = null)
        {
            JoinRoomRequested?.Invoke(room, password);
            base.JoinRoom(room, password, onSuccess, onError);
        }

        public void AddRooms(int count, RulesetInfo? ruleset = null, bool withPassword = false, bool withSpotlightRooms = false)
        {
            for (int i = 0; i < count; i++)
            {
                AddRoom(new Room
                {
                    Name = $@"Room {currentRoomId}",
                    Host = new APIUser { Username = @"Host" },
                    Duration = TimeSpan.FromSeconds(10),
                    Category = withSpotlightRooms && i % 2 == 0 ? RoomCategory.Spotlight : RoomCategory.Normal,
                    Password = withPassword ? @"password" : null,
                    PlaylistItemStats = ruleset == null
                        ? null
                        : new Room.RoomPlaylistItemStats { RulesetIDs = [ruleset.OnlineID] },
                    Playlist = ruleset == null
                        ? Array.Empty<PlaylistItem>()
                        : [new PlaylistItem(new BeatmapInfo { Metadata = new BeatmapMetadata() }) { RulesetID = ruleset.OnlineID }]
                });
            }
        }

        public void AddRoom(Room room)
        {
            room.RoomID = -currentRoomId;
            CreateRoom(room);
            currentRoomId++;
        }
    }
}
