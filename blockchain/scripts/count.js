const { ethers } = require("hardhat");

async function main() {
    // Get contract address from environment variable or command line argument
    const contractAddress = '0xbaAA2a3237035A2c7fA2A33c76B44a8C6Fe18e87';
    const reminder = 'test-reminder';

    const Reminders = await ethers.getContractFactory("Reminders");
    const reminders = Reminders.attach(contractAddress);

    // Call the getReminderCount function
    const count = await reminders.getReminderCount();

    console.log("Reminder count:", count);
}

main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
});
