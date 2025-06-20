import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";

import {
  addUserToProcedure,
  removeUserFromProcedure,
  getProcedureUsers
} from "../../../api/api";

const PlanProcedureItem = ({ procedure, users }) => {
    const [selectedUsers, setSelectedUsers] = useState(null);

    useEffect(() => {
        (async () => {
            var procedureUsers = await getProcedureUsers(procedure.procedureId);
            if(procedureUsers)
            {
                const usersval = procedureUsers.map(item => ({value: item.user.userId, label: item.user.name}));
                setSelectedUsers(usersval);
            }
        })();
    }, [procedure.procedureId]);
    
    const handleAssignUserToProcedure = async (e) => {
        var removed = e.length > 0 ? selectedUsers.filter(item => !e.includes(item)) : null;
        var added = e.filter(item => !selectedUsers.includes(item));
        setSelectedUsers(e);
        if(added && added.length > 0)
        {
            await addUserToProcedure(procedure.procedureId, added[0].value);
        }
        else {
            await removeUserFromProcedure(procedure.procedureId,
                 removed && removed.length > 0 ? removed[0].value : -1);
        }
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleAssignUserToProcedure(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;
