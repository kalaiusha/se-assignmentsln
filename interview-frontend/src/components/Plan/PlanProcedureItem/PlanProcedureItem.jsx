import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";

import {
  addUserToProcedure,
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
        setSelectedUsers(e);
        const usersvalue = e.map(item => item.value);
        await addUserToProcedure(procedure.procedureId, usersvalue);
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
