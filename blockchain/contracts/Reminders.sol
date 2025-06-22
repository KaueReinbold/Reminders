// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract Reminders {
    struct Reminder {
        uint256 id;
        string text;
        address owner;
        bool exists;
    }

    mapping(uint256 => Reminder) public reminders;
    uint256 public nextId;

    event ReminderCreated(uint256 id, address owner, string text);
    event ReminderUpdated(uint256 id, string text);
    event ReminderDeleted(uint256 id);

    function createReminder(string calldata text) external {
        reminders[nextId] = Reminder(nextId, text, msg.sender, true);
        emit ReminderCreated(nextId, msg.sender, text);
        nextId++;
    }

    function updateReminder(uint256 id, string calldata text) external {
        validateOwner(id);

        reminders[id].text = text;

        emit ReminderUpdated(id, text);
    }

    function deleteReminder(uint256 id) external {
        validateOwner(id);

        delete reminders[id];

        emit ReminderDeleted(id);
    }

    function getReminder(
        uint256 id
    ) external view returns (string memory, address) {
        validateNotFound(id);

        return (reminders[id].text, reminders[id].owner);
    }

    function getReminderCount() external view returns (uint256) {
        return nextId;
    }

    function validateNotFound(uint256 id) private view {
        require(reminders[id].exists, "Not found");
    }

    function validateOwner(uint256 id) private view {
        validateNotFound(id);

        require(reminders[id].owner == msg.sender, "Not owner");
    }
}
