local checkpointposition = nil

registertalkactionsplayersay("/cp", function(player, message)
	if cast("System.Int64", player.Rank) == rank.gamemaster then
		checkpointposition = player.Tile.Position
		command.showmagiceffect(checkpointposition, magiceffecttype.blueshimmer)
		return true -- handled, stop process
	end
	return false
end)

registertalkactionsplayersay("/tp", function(player, message)
	if cast("System.Int64", player.Rank) == rank.gamemaster then
		if checkpointposition then
			local position = player.Tile.Position:Offset(player.Direction)
			local tile = command.mapgettile(position)
			if tile then
				local item = command.tilecreateitem(tile, 1387, 0)
				item.Position = checkpointposition
				command.showmagiceffect(position, magiceffecttype.blueshimmer)
				local space = string.find(message, " ")
				if space then
					local argument = string.sub(message, space + 1) -- 1
					message = string.sub(message, 1, space - 1) -- /tp
					local seconds = tonumber(argument)
					if seconds and seconds > 0 then
						function loop(seconds)
							if seconds > 0 then						
								command.showanimatedtext(position, animatedtextcolor.blue, seconds)
								command.delay(item, 1 * 1000, function()
									loop(seconds - 1)
								end)
							else
								command.showmagiceffect(position, magiceffecttype.puff)
								command.itemdestroy(item)
							end
						end
						loop(seconds)
					end
				end
			else
				command.showmagiceffect(position, magiceffecttype.puff)
			end
		else
			command.showwindowtext(player, messagemode.failure, "Use /cp to set a checkpoint then /tp to create a teleport")
			command.showmagiceffect(position, magiceffecttype.puff)
		end
		return true -- handled, stop process
	end
	return false
end)