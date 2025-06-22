import { ethers } from "hardhat";
import { expect } from "chai";

describe("Reminders", function () {
    it("should create, update, and delete a reminder", async function () {
        const Reminders = await ethers.getContractFactory("Reminders");
        const reminders = await Reminders.deploy();

        await reminders.waitForDeployment();

        const [owner, other] = await ethers.getSigners();

        // Create reminder
        await reminders.createReminder("First reminder");

        let reminder = await reminders.getReminder(0);

        expect(reminder[0]).to.equal("First reminder");
        expect(reminder[1]).to.equal(owner.address);

        // Count
        expect(await reminders.getReminderCount()).to.equal(1);

        // Update reminder
        await reminders.updateReminder(0, "Updated reminder");

        reminder = await reminders.getReminder(0);

        expect(reminder[0]).to.equal("Updated reminder");

        // Only owner can delete
        await expect(
            reminders.connect(other).deleteReminder(0)
        ).to.be.rejectedWith("Not owner");

        // Delete reminder
        await reminders.deleteReminder(0);
        await expect(reminders.getReminder(0)).to.be.rejectedWith("Not found");
    });
});