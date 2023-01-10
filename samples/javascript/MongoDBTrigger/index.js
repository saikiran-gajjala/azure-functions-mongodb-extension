module.exports = async function (context, eventData) {
    context.log.info(`JavaScript MongoDB trigger function called for message ${JSON.stringify(eventData)}}`);
};